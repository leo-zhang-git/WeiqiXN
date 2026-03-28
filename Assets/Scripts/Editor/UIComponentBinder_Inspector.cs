using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[CustomEditor(typeof(UIComponentBinder))]
public class UIComponentBinder_Inspector : Editor
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
        typeof(RectTransform),
    };

    private UIComponentBinder instance;
    private bool isEditable;

    private Dictionary<string, int> nameBindDict = new Dictionary<string, int>();
    private Dictionary<Object, int> objectBindDict = new Dictionary<Object, int>();

    private string filterName;
    private GameObject filterGO;

    private void OnEnable()
    {
        instance = target as UIComponentBinder;
        isEditable = CheckBinderEditAble();
    }

    public override void OnInspectorGUI()
    {
        nameBindDict.Clear();
        objectBindDict.Clear();
        foreach (var node in instance.nodeList) {
            if (nameBindDict.TryGetValue(node.name, out int nameBindTimes)) {
                nameBindDict[node.name] = nameBindTimes + 1;
            } else {
                nameBindDict.Add(node.name, 1);
            }

            if (objectBindDict.TryGetValue(node.value, out int objectBindTimes)) {
                objectBindDict[node.value] = objectBindTimes + 1;
            } else {
                objectBindDict.Add(node.value, 1);
            }
        }

        if (isEditable) {
            using (new EditorGUILayout.HorizontalScope()) {
                bool isExists = File.Exists(instance.binderExportPath);
                EditorGUILayout.TextArea($"※生成路径:({(isExists ? "已生成" : "未生成")}) \n{instance.binderExportPath}", EditorStyles.wordWrappedLabel);
                using (new EditorGUILayout.VerticalScope()) {
                    if (GUILayout.Button("生成类文件", GUILayout.Width(100))) {
                        foreach (var kvp in nameBindDict) {
                            if (kvp.Value > 1) {
                                EditorUtility.DisplayDialog("导出错误", "存在重复名字，检查UI绑定脚本！", "确定");
                                return;
                            }
                        }

                        bool doExport = true;
                        var prefabStage = PrefabStageUtility.GetPrefabStage(instance.gameObject);
                        if (prefabStage && prefabStage.scene.isDirty) {
                            doExport = EditorUtility.DisplayDialog("未保存修改", "是否保存并执行UI绑定脚本生成？", "确定", "取消");
                            if (doExport) {
                                EditorApplication.ExecuteMenuItem("File/Save");
                            }
                        }

                        if (doExport) {
                            UIGenerator.ExportUIScripts(instance);
                            UIBinderBase logicBinder = instance.GetComponent<UIBinderBase>();
                            if (logicBinder != null) {
                                DestroyImmediate(logicBinder);
                            }

                            // 自动绑定要处理编译时序问题太麻烦了，改成手动点击绑定
                            EditorUtility.DisplayDialog("UI绑定", $"成功生成UI绑定文件，重编译后执行更新绑定：\n{instance.binderExportPath}",
                                "确定");
                        }
                    }

                    var uiBinderTypes = TypeCache.GetTypesDerivedFrom<UIBinderBase>();
                    Type binderType = uiBinderTypes.FirstOrDefault(t => t.Name == instance.binderClsName);
                    if (binderType != null) {
                        if (GUILayout.Button("更新绑定", GUILayout.Width(100))) {
                            var attachBinder = instance.gameObject.GetComponent(binderType);
                            if (attachBinder == null) {
                                attachBinder = instance.gameObject.AddComponent(binderType);
                            }
                            if (attachBinder != null) {
                                var attachBinderFields = attachBinder.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                                foreach (var node in instance.nodeList) {
                                    if (string.IsNullOrEmpty(node.name)) {
                                        continue;
                                    }
                                    var field = attachBinderFields.FirstOrDefault(f => string.Equals(f.Name, node.name, StringComparison.OrdinalIgnoreCase));
                                    if (field == null) {
                                        continue;
                                    }

                                    if (field.FieldType == node.value.GetType()) {
                                        field.SetValue(attachBinder, node.value);
                                    }
                                }
                            }

                            EditorUtility.SetDirty(target);
                            serializedObject.ApplyModifiedProperties();
                            EditorUtility.DisplayDialog("UI绑定", "UI绑定已更新", "确定");
                        }
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

        EditorGUILayout.Space();
        if (instance.isNodesExpand = EditorGUILayout.Foldout(instance.isNodesExpand, $"Components({instance.nodeList.Count})")) {
            using (new EditorGUI.DisabledScope(!isEditable)) {
                using (new EditorGUI.IndentLevelScope()) {
                    for (int index = 0; index < instance.nodeList.Count; index++) {
                        using (new EditorGUILayout.HorizontalScope()) {
                            DrawUIBinderNode(index, instance.nodeList[index]);
                        }
                    }
                }

                using (var check = new EditorGUI.ChangeCheckScope()) {
                    EditorGUILayout.Separator();
                    EditorGUILayout.ObjectField(null, typeof(GameObject), true, GUILayout.Height(80));

                    if (check.changed) {
                        var selectionGOS = DragAndDrop.objectReferences;
                        if (selectionGOS.Any(obj => PrefabUtility.IsPartOfPrefabAsset(obj))) {
                            EditorUtility.DisplayDialog("UI绑定错误", "不允许绑定Preafab Asset", "确定");
                        } else if (selectionGOS.Length > 0) {
                            Undo.RecordObject(target, "BIND_CHANGE");
                            foreach (var obj in selectionGOS) {
                                Object nodeValue;
                                if (obj != instance.gameObject && AutoFetchBindComponent(obj as GameObject, out var comp)) {
                                    nodeValue = comp;
                                } else {
                                    nodeValue = obj;
                                }

                                string nodeName = char.ToLower(obj.name[0]) + obj.name.Substring(1); // 首字母小写 
                                instance.nodeList.Add(new UIBinderNode(nodeName, nodeValue));
                            }
                            EditorUtility.SetDirty(target);
                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
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
        bindComp = bindGO.GetComponent<UIComponentBinder>();
        if (bindComp != null) {
            return true;
        }

        foreach (var compType in ValidComponentTypes) {
            if (bindGO.TryGetComponent(compType, out bindComp)) {
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
        var binder = go.GetComponent<UIComponentBinder>();
        if (binder != null) {
            compList.Add(binder);
        }
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
        Color overrideColor = objectBindDict.GetValueOrDefault(node.value, 0) > 1 ? Color.yellow : GUI.color;
        Color nameColor = nameBindDict.GetValueOrDefault(node.name, 0) > 1 ? Color.red : overrideColor;
        using (var check = new EditorGUI.ChangeCheckScope()) {
            string nodeName;
            using (new EditorColorScope(nameColor)) {
                GUILayout.Label(index.ToString(), GUILayout.Width(index.ToString().Length * 11));
                GUILayout.Space(-index.ToString().Length * 5);
                nodeName = EditorGUILayout.TextField(node.name, GUILayout.Width(125));
            }

            if (check.changed) {
                Undo.RecordObject(target, "BIND_CHANGE");
                node.name = nodeName;
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
        }

        Color valueColor = !node.value || objectBindDict.GetValueOrDefault(node.value, 0) > 1 ? Color.red : overrideColor;
        using (var check = new EditorGUI.ChangeCheckScope()) {
            Object nodeValue;
            using (new EditorColorScope(GUI.color)) {
                nodeValue = EditorGUILayout.ObjectField(node.value, typeof(Object), true, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true)) as Object;
            }

            if (check.changed) {
                if (PrefabUtility.IsPartOfPrefabAsset(nodeValue)) {
                    EditorUtility.DisplayDialog("UI绑定错误", "不允许绑定Preafab Asset", "确定");
                } else {
                    Undo.RecordObject(target, "BIND_CHANGE");
                    if (nodeValue is GameObject bindGO) {
                        if (nodeValue != instance.gameObject && AutoFetchBindComponent(bindGO, out var bindComp)) {
                            node.value = bindComp;
                        } else {
                            node.value = nodeValue;
                        }
                    } else {
                        node.value = nodeValue;
                    }

                    EditorUtility.SetDirty(target);
                    serializedObject.ApplyModifiedProperties();
                }
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
