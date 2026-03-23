using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[CustomEditor(typeof(UIBinderGenerator))]
public class UIBinderGenerator_Inspector : Editor
{
    private static Type[] ValidComponentTypes = new[]
    {
        typeof(Animator),
        typeof(Button),
        typeof(TMP_InputField),
        typeof(TextMeshProUGUI),
        typeof(Text),
        typeof(Image),
        typeof(RawImage),
    };

    private UIBinderGenerator instance;
    private bool isEditable;
    private string generatePath = "test";

    private string filterName;
    private GameObject filterGO;

    private void OnEnable()
    {
        instance = target as UIBinderGenerator;
        isEditable = CheckBinderEditAble();
    }

    public override void OnInspectorGUI()
    {
        if (isEditable) {
            using (new EditorGUILayout.HorizontalScope()) {
                bool isExists = File.Exists(generatePath);
                EditorGUILayout.TextArea($"※生成路径:({(isExists ? "已生成" : "未生成")}) \n{generatePath}", EditorStyles.wordWrappedLabel);
                if (GUILayout.Button("生成类文件", GUILayout.Width(100))) {
                    bool doExport = true;
                    var prefabStage = PrefabStageUtility.GetPrefabStage(instance.gameObject);
                    if (prefabStage && prefabStage.scene.isDirty) {
                        doExport = EditorUtility.DisplayDialog("未保存修改", "是否保存并执行UI绑定脚本生成？", "确定", "取消");
                        if (doExport) {
                            EditorApplication.ExecuteMenuItem("File/Save");
                        }
                    }

                    if (doExport) {
                        // TODO

                        EditorApplication.delayCall += () =>
                        {
                            EditorUtility.DisplayDialog("UI绑定脚本生成", $"成功生成UI绑定文件：\n{generatePath}",
                                "确定");
                        };
                    }
                }
            }
            GUILayout.Space(10f);
        }

        using (new EditorGUI.DisabledScope(filterGO != null)) {
            filterName = EditorGUILayout.TextField("按名字筛选", filterName);
        }
        using (new EditorGUI.DisabledScope(!string.IsNullOrEmpty(filterName))) {
            filterGO = EditorGUILayout.ObjectField("按GO筛选", filterGO, typeof(GameObject), true) as GameObject;
        }
        if (GUILayout.Button("清空筛选", GUILayout.Width(80))) {
            filterName = string.Empty;
            filterGO = null;
        }

        using (new EditorGUI.DisabledScope(!isEditable)) {
            using (new EditorGUI.IndentLevelScope()) {
                for (int index = 0; index < instance.nodeList.Count; index++) {
                    using (new EditorGUILayout.HorizontalScope()) {
                        DrawUIBinderNode(index, instance.nodeList[index]);
                    }
                }
            }
        }
    }

    private bool CheckBinderEditAble()
    {
        if (instance.gameObject.GetComponent<RectTransform>() == null) {
            return false;
        }

        // 要求prefab编辑模式下的根节点才可编辑
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null) {
            if (prefabStage.prefabContentsRoot == instance.gameObject) {
                return true;
            }
        }

        return false;
    }

    private bool AutoFetchBindComponent(GameObject bindGO, out Component bindComp)
    {
        bindComp = null;

        foreach (var compType in ValidComponentTypes) {
            bindComp = bindGO.GetComponent(compType);
            if (bindComp != null) {
                return true;
            }
        }

        return false;
    }

    private List<Object> FetchValidComponents(Object obj)
    {
        GameObject go = obj switch
        {
            Component component => component.gameObject,
            GameObject gameObject => gameObject,
            _ => null
        };

        List<Object> compList = new List<Object>() { go };
        foreach (var comp in go.GetComponents<Component>()) {
            if (ValidComponentTypes.Contains(comp.GetType())) {
                compList.Add(comp);
            }
        }

        compList.Sort((compA, compB) => String.Compare(compA.GetType().Name, compB.GetType().Name, StringComparison.Ordinal));
        return compList;
    }

    private void DrawUIBinderNode(int index, UIBinderNode node)
    {
        // TODO override color
        GUILayout.Label(index.ToString(), GUILayout.Width(index.ToString().Length * 10));
        GUILayout.Space(-index.ToString().Length * 5);
        node.name = EditorGUILayout.TextField(node.name, GUILayout.Width(150));

        using (var check = new EditorGUI.ChangeCheckScope()) {
            GameObject bindGO = EditorGUILayout.ObjectField(node.value, typeof(GameObject), true, GUILayout.MinWidth(50), GUILayout.ExpandWidth(true)) as GameObject;

            if (check.changed) {
                if (PrefabUtility.IsPartOfPrefabAsset(bindGO)) {
                    Logger.LogError("Prefab is not allowed to set in ui binder generator");
                } else {
                    Undo.RecordObject(target, "BIND_CHANGE");
                    if (bindGO != instance.gameObject && AutoFetchBindComponent(bindGO, out var bindComp)) {
                        node.value = bindComp;
                    } else {
                        node.value = bindGO;
                    }

                    EditorUtility.SetDirty(target);
                    serializedObject.ApplyModifiedProperties();
                }
            }

            if (EditorGUILayout.DropdownButton(GUIContent.none, FocusType.Passive, GUILayout.Width(20))) {
                GenericMenu menu = new GenericMenu();

                var validComps = FetchValidComponents(node.value);
                GenericMenu.MenuFunction2 onSelected = (selectedObj) =>
                {
                    Undo.RecordObject(target, "BIND_Change");
                    node.value = selectedObj as Object;

                    EditorUtility.SetDirty(target);
                    serializedObject.ApplyModifiedProperties();
                };

                Dictionary<string, int> duplicatedCompDict = new Dictionary<string, int>();
                foreach (Object comp in validComps) {
                    string objName = comp.GetType().Name;

                    int count = duplicatedCompDict.GetValueOrDefault(objName, 0) + 1;
                    duplicatedCompDict[objName] = count;

                    string displayName = count > 1 ? $"{objName}({count - 1})" : objName;
                    menu.AddItem(new GUIContent(displayName), false, onSelected, comp);
                }
                menu.ShowAsContext();
            }

            if (GUILayout.Button("删除", GUILayout.Width(80))) {
                Undo.RecordObject(target, "BIND_CHANGE");
                instance.nodeList.RemoveAt(index);
                EditorUtility.SetDirty(target);
            }
        }
    }
}
