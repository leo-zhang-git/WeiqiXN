using System;
using System.Collections.Generic;
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

public class UIComponentBinder : MonoBehaviour
{
    public Type attachType;
    public List<UIBinderNode> nodeList = new List<UIBinderNode>();
    public bool isNodesExpand = true;
}