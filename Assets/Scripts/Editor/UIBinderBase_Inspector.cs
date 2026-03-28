using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIBinderBase), true)]
public class UIBinderBase_Inspector : Editor
{
    private UIBinderBase instance;
    private MonoScript[] projectScripts;

    private void OnEnable()
    {
        instance = target as UIBinderBase;
        projectScripts = Resources.FindObjectsOfTypeAll<MonoScript>();
    }

    public override void OnInspectorGUI()
    {
        UIComponentBinder uiCompBinder = instance.GetComponent<UIComponentBinder>();
        if (uiCompBinder != null && uiCompBinder.generateTime > instance.generatedTime) {
            EditorGUILayout.HelpBox($"绑定文件已过期，需要更新绑定", MessageType.Warning);
        }

        var binderFields = instance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        using (new EditorGUI.DisabledScope(true)) {
            EditorGUILayout.TextArea("常规UI对象：", EditorStyles.wordWrappedLabel);
            List<(string, UIComponentBinder)> childBinders = new List<(string, UIComponentBinder)>();
            foreach (var field in binderFields) {
                if (field.GetValue(instance) is UIComponentBinder binder) {
                    childBinders.Add((field.Name.Substring(1), binder));
                } else if (field.GetValue(instance) is Object objValue) {
                    using (new EditorGUILayout.HorizontalScope()) {
                        EditorGUILayout.TextArea("对象名", EditorStyles.boldLabel, GUILayout.Width(125));
                        EditorGUILayout.TextArea("绑定对象", EditorStyles.boldLabel, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
                    }
                    DrawUIBindNode(field.Name, objValue);
                }
            }

            if (childBinders.Count > 0) {
                EditorGUILayout.TextArea("WidgetUI对象：", EditorStyles.wordWrappedLabel);
                using (new EditorGUILayout.HorizontalScope()) {
                    EditorGUILayout.TextArea("对象名", EditorStyles.boldLabel, GUILayout.Width(125));
                    EditorGUILayout.TextArea("绑定对象", EditorStyles.boldLabel, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
                    EditorGUILayout.TextArea("逻辑脚本", EditorStyles.boldLabel, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
                }
                foreach (var (name, binder) in childBinders) {
                    DrawWidgetBindNode(name, binder);
                }
            }
        }
    }

    private void DrawUIBindNode(string objName, Object objValue)
    {
        using (new EditorGUILayout.HorizontalScope()) {
            EditorGUILayout.TextField(objName, GUILayout.Width(125));
            EditorGUILayout.ObjectField(objValue, typeof(Object), true, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
        }
    }

    private void DrawWidgetBindNode(string objName, UIComponentBinder binder)
    {
        using (new EditorGUILayout.HorizontalScope()) {
            EditorGUILayout.TextArea(objName, GUILayout.Width(125));
            EditorGUILayout.ObjectField(binder, typeof(UIComponentBinder), true, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            var widgetScript = projectScripts.FirstOrDefault(s => s.GetClass() != null && s.GetClass().Name == binder.logicClsName);
            EditorGUILayout.ObjectField(widgetScript, typeof(MonoScript), true, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
        }
    }
}
