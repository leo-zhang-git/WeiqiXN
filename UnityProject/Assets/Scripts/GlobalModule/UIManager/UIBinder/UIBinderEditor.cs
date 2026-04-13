using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class UIBinderNode
{
    public string name;
    public Object value;

    public UIBinderNode(string name, Object value)
    {
        this.name = name;
        this.value = value;
    }
}

public class UIBinderEditor : MonoBehaviour
{
    private const string GO_CLONE_SUFFIX = "(Clone)";

    public long generateTime;
    public Type attachType;
    public List<UIBinderNode> nodeList = new List<UIBinderNode>();
    public bool isNodesExpand = true;

    private string baseName
    {
        get
        {
            string goName = gameObject.name;
            if (goName.EndsWith(GO_CLONE_SUFFIX, StringComparison.Ordinal)) {
                return goName.Substring(0, goName.Length - GO_CLONE_SUFFIX.Length);
            }

            return goName;
        }
    }

    public string binderClsName => $"{baseName}UI";
    public string logicClsName => baseName;
    public bool isPage => gameObject.GetComponent<Canvas>() != null;
    public string binderExportPath => Path.GetFullPath(Path.Combine(UIConfig.PATH_UI_BINDER_EXPORT, isPage ? "Page" : "Widget", $"{binderClsName}.cs"));
    public string logicExportPath => Path.GetFullPath(Path.Combine(UIConfig.PATH_UI_LOGIC_EXPORT, isPage ? "Page" : "Widget", $"{logicClsName}.cs"));
}
