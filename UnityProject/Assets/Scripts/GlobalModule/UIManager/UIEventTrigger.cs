using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventTrigger : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IScrollHandler
{
    public Action<PointerEventData> onBeginDragHandler;
    public Action<PointerEventData> onDragHandler;
    public Action<PointerEventData> onEndDragHandler;
    public Action<PointerEventData> onPointerClickHandler;
    public Action<PointerEventData> onPointerDownHandler;
    public Action<PointerEventData> onPointerEnterHandler;
    public Action<PointerEventData> onPointerExitHandler;
    public Action<PointerEventData> onPointerUpHandler;
    public Action<PointerEventData> onScrollHandler;

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDragHandler?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDragHandler?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDragHandler?.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onPointerClickHandler?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDownHandler?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnterHandler?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExitHandler?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUpHandler?.Invoke(eventData);
    }

    public void OnScroll(PointerEventData eventData)
    {
        onScrollHandler?.Invoke(eventData);
    }

    public void RemoveOnBeginDragHandler()
    {
        onBeginDragHandler = null;
    }

    public void RemoveOnDragHandler()
    {
        onDragHandler = null;
    }

    public void RemoveOnEndDragHandler()
    {
        onEndDragHandler = null;
    }

    public void RemoveOnPointerClickHandler()
    {
        onPointerClickHandler = null;
    }

    public void RemoveOnPointerDownHandler()
    {
        onPointerDownHandler = null;
    }

    public void RemoveOnPointerEnterHandler()
    {
        onPointerEnterHandler = null;
    }

    public void RemoveOnPointerExitHandler()
    {
        onPointerExitHandler = null;
    }

    public void RemoveOnPointerUpHandler()
    {
        onPointerUpHandler = null;
    }

    public void RemoveOnScrollHandler()
    {
        onScrollHandler = null;
    }
}
