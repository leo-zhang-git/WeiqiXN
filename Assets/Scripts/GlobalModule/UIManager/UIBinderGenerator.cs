using System.Collections.Generic;
using UnityEngine;

[SerializeField]
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

public class UIBinderGenerator : MonoBehaviour
{
    public List<UIBinderNode> nodeList = new List<UIBinderNode>();
}