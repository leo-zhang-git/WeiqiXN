using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIBinderEditor))]
public abstract class UIBinderBase : MonoBehaviour
{
    public long generatedTime;
    public Dictionary<string, UIWidget> binderWidgets = new Dictionary<string, UIWidget>();
    public Dictionary<string, GameObject> binderWidgetGOs = new Dictionary<string, GameObject>();

    public virtual void InitWidgets(UILogicBase owner)
    {
        binderWidgets.Clear();
    }
}
