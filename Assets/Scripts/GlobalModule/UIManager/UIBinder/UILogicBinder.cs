using UnityEngine;

[RequireComponent(typeof(UIComponentBinder))]
public abstract class UILogicBinder : MonoBehaviour
{
    public abstract string logicClsName { get; }
}
