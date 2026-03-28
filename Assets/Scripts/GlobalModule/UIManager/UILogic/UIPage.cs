using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPage : UILogicBase
{
    public UIContext pageContext;
    public int canvasOrder;
    [Flags]
    public enum PageFlags
    {
        None = 0,
        MainPage = 1,
        PopupPage = 2,
    }
    public PageFlags pageFlags;
    public bool isMainPage => (pageFlags & PageFlags.MainPage) == PageFlags.MainPage;
    public bool isPopupPage => (pageFlags & PageFlags.PopupPage) == PageFlags.PopupPage;

    protected List<UIWidget> childWidgets = new List<UIWidget>();

    private Canvas _canvas;
    public Canvas canvas
    {
        get
        {
            if (!isLoaded) {
                Logger.LogError("Try get canvas before ui resorece is loaded.", ("pageName", GetType().Name));
                return null;
            }
            if (_canvas == null) {
                _canvas = gameObject.GetComponent<Canvas>();
            }
            return _canvas;
        }
    }

    protected override void OnLoaded()
    {
        base.OnLoaded();
        canvas.sortingOrder = pageContext.baseCanvasOrder;
    }

    protected override void OnClose()
    {
        foreach (var widget in childWidgets) {
            widget.CloseWidget();
        }

        base.OnClose();
    }

    public void LoadPage()
    {

    }

    public void ClosePage()
    {

    }
}

public abstract class UIPageWithBinder<TBinder> : UIPage where TBinder : UIBinderBase
{
    public TBinder binder;

    protected override void OnLoaded()
    {
        base.OnLoaded();
        binder = gameObject.GetComponent<TBinder>();
        binder.InitWidgets();

        foreach (var widget in binder.binderWidgets) {
            childWidgets.Add(widget);
        }
    }
}
