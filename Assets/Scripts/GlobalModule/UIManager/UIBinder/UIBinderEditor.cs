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
    public DateTime generateTime;
    public Type attachType;
    public List<UIBinderNode> nodeList = new List<UIBinderNode>();
    public bool isNodesExpand = true;

    public string binderClsName => $"{gameObject.name}UI";
    public string logicClsName => gameObject.name;
    public bool isPage => gameObject.GetComponent<Canvas>() != null;
    public string binderExportPath => Path.GetFullPath(Path.Combine(UIConfig.PATH_UI_BINDER_EXPORT, isPage ? "Page" : "Widget", $"{binderClsName}.cs"));
    public string logicExportPath => Path.GetFullPath(Path.Combine(UIConfig.PATH_UI_LOGIC_EXPORT, isPage ? "Page" : "Widget", $"{logicClsName}.cs"));
}