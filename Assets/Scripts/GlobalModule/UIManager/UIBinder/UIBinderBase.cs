using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIComponentBinder))]
public abstract class UIBinderBase : MonoBehaviour
{
    public DateTime generatedTime;
    public List<UIWidget> binderWidgets = new List<UIWidget>();

    public virtual void InitWidgets()
    {

    }
}
