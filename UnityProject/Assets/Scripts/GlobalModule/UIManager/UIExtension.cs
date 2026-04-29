using System;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UIExtension
{
    public static Action<PointerEventData> WrapPointerEventAction(Action action)
    {
        return (PointerEventData evt) =>
        {
            action.Invoke();
        };
    }

    public static void AddBeginDragListener(this MaskableGraphic graphic, Action<PointerEventData> callback)
    {
        UIEventTrigger trigger = graphic.GetOrAddComponent<UIEventTrigger>();
        trigger.onBeginDragHandler = callback;
    }

    public static void RemoveBeginDragListener(this MaskableGraphic graphic)
    {
        UIEventTrigger trigger = graphic.GetComponent<UIEventTrigger>();
        if (trigger != null) {
            trigger.RemoveOnBeginDragHandler();
        }
    }

    public static void AddDragListener(this MaskableGraphic graphic, Action<PointerEventData> callback)
    {
        UIEventTrigger trigger = graphic.GetOrAddComponent<UIEventTrigger>();
        trigger.onDragHandler = callback;
    }

    public static void RemoveDragListener(this MaskableGraphic graphic)
    {
        UIEventTrigger trigger = graphic.GetComponent<UIEventTrigger>();
        if (trigger != null) {
            trigger.RemoveOnDragHandler();
        }
    }

    public static void AddEndDragListener(this MaskableGraphic graphic, Action<PointerEventData> callback)
    {
        UIEventTrigger trigger = graphic.GetOrAddComponent<UIEventTrigger>();
        trigger.onEndDragHandler = callback;
    }

    public static void RemoveEndDragListener(this MaskableGraphic graphic, Action<PointerEventData> callback)
    {
        UIEventTrigger trigger = graphic.GetComponent<UIEventTrigger>();
        if (trigger != null) {
            trigger.RemoveOnEndDragHandler();
        }
    }

    public static void AddOnPointerClickListener(this MaskableGraphic graphic, Action callback)
    {
        UIEventTrigger trigger = graphic.GetOrAddComponent<UIEventTrigger>();
        trigger.onPointerClickHandler = WrapPointerEventAction(callback);
    }

    public static void RemoveOnPointerClickListener(this MaskableGraphic graphic)
    {
        UIEventTrigger trigger = graphic.GetComponent<UIEventTrigger>();
        if (trigger != null) {
            trigger.RemoveOnPointerClickHandler();
        }
    }

    public static void AddOnPointerDownListener(this MaskableGraphic graphic, Action callback)
    {
        UIEventTrigger trigger = graphic.GetOrAddComponent<UIEventTrigger>();
        trigger.onPointerDownHandler = WrapPointerEventAction(callback);
    }

    public static void RemoveOnPointerDownListener(this MaskableGraphic graphic)
    {
        UIEventTrigger trigger = graphic.GetOrAddComponent<UIEventTrigger>();
        if (trigger != null) {
            trigger.RemoveOnPointerDownHandler();
        }
    }

    public static void AddOnPointerEnterListener(this MaskableGraphic graphic, Action callback)
    {
        UIEventTrigger trigger = graphic.GetOrAddComponent<UIEventTrigger>();
        trigger.onPointerEnterHandler = WrapPointerEventAction(callback);
    }

    public static void RemoveOnPointerEnterListener(this MaskableGraphic graphic)
    {
        UIEventTrigger trigger = graphic.GetOrAddComponent<UIEventTrigger>();
        if (trigger != null) {
            trigger.RemoveOnPointerEnterHandler();
        }
    }

    public static void AddOnPointerExitListener(this MaskableGraphic graphic, Action callback)
    {
        UIEventTrigger trigger = graphic.GetOrAddComponent<UIEventTrigger>();
        trigger.onPointerExitHandler = WrapPointerEventAction(callback);
    }

    public static void RemoveOnPointerExitListener(this MaskableGraphic graphic)
    {
        UIEventTrigger trigger = graphic.GetComponent<UIEventTrigger>();
        if (trigger != null) {
            trigger.RemoveOnPointerExitHandler();
        }
    }

    public static void AddOnPointerUpListener(this MaskableGraphic graphic, Action callback)
    {
        UIEventTrigger trigger = graphic.GetOrAddComponent<UIEventTrigger>();
        trigger.onPointerUpHandler = WrapPointerEventAction(callback);
    }

    public static void RemoveOnPointerUpListener(this MaskableGraphic graphic, Action callback)
    {
        UIEventTrigger trigger = graphic.GetComponent<UIEventTrigger>();
        if (trigger != null) {
            trigger.RemoveOnPointerUpHandler();
        }
    }

    public static void AddOnScrollListener(this MaskableGraphic graphic, Action<PointerEventData> callback)
    {
        UIEventTrigger trigger = graphic.GetOrAddComponent<UIEventTrigger>();
        trigger.onScrollHandler = callback;
    }

    public static void RemoveOnScrollListener(this MaskableGraphic graphic)
    {
        UIEventTrigger trigger = graphic.GetComponent<UIEventTrigger>();
        if (trigger != null) {
            trigger.RemoveOnScrollHandler();
        }
    }
}
