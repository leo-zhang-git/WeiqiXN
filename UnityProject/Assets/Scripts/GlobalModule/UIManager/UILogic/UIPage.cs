using System.Collections.Generic;
using UnityEngine;

public abstract class UIPage : UILogicBase
{
    public UIContext owner;
    private int _canvasOrder;
    public int canvasOrder
    {
        get
        {
            return _canvasOrder;
        }
        set
        {
            if (isLoaded) {
                canvas.sortingOrder = value;
            } else {
                AddResourceLoadedCB(() =>
                {
                    canvas.sortingOrder = value;
                });
            }
            _canvasOrder = value;
        }
    }
    public UiPageDataType pageConfig
    {
        get
        {
            return UiPageDataType.GetConfigData(pageName);
        }
    }

    public abstract string pageName { get; }
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

    public static string GetPageName<TPage>() where TPage : UIPage
    {
        return typeof(TPage).Name;
    }

    public static TPage CreatePageInstance<TPage>(UIContext owner) where TPage : UIPage, new()
    {
        TPage page = new TPage();
        page.owner = owner;
        return page;
    }

    public void InitPage(UIContext owner)
    {
        this.owner = owner;
    }

    protected override void OnLoaded()
    {
        base.OnLoaded();

        var uiCamera = Global.Instance.uiManager.uiCamera;
        if (uiCamera != null) {
            canvas.worldCamera = uiCamera;
        }
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        if (!pageConfig.isPopup) {
            if (owner.mainPageStack.Count > 1 && owner.mainPageStack.Last.Value == this) {
                var previousPage = owner.mainPageStack.Last.Previous.Value;
                previousPage.SetUIVisible(false);
                SetUIVisible(true);
            }
        } else {
            SetUIVisible(true);
        }
    }

    protected override void OnClose()
    {
        foreach (var widget in childWidgets) {
            widget.CloseWidget();
        }

        base.OnClose();
    }

    public override void SetUIVisible(bool isVisible)
    {
        base.SetUIVisible(isVisible);
        canvas.enabled = isVisible;

        foreach (var widget in childWidgets) {
            widget.SetUIVisible(isVisible);
        }
    }

    public void LoadPage()
    {
        if (Global.Instance.uiManager.TryGetCachePageGO(pageName, out GameObject pageGO)) {
            pageGO.SetActive(true);
            onUnityResourceLoaded(pageGO);
        } else {
            string assetPath = UIUtils.GetPagePrefabPath(pageName);
            bool isAsync = pageConfig == null || pageConfig.isLoadAsync;
            if (isAsync) {
                Global.Instance.resourceManager.LoadGamePrefabAsync(this, assetPath, onUnityResourceLoaded);
            } else {
                pageGO = Global.Instance.resourceManager.LoadGamePrefab(assetPath);
                if (pageGO != null) {
                    onUnityResourceLoaded(pageGO);
                }
            }
        }
    }

    public void ClosePage()
    {
        foreach (var widget in childWidgets) {
            widget.CloseWidget();
        }
        OnHide();
        Global.Instance.uiManager.ClosePage(this);
        OnClose();
    }
}

public abstract class UIPageWithBinder<TBinder> : UIPage where TBinder : UIBinderBase
{
    public TBinder binder;

    protected override void OnLoaded()
    {
        base.OnLoaded();
        binder = gameObject.GetComponent<TBinder>();
        binder.InitWidgets(this);

        foreach (var widgetKV in binder.binderWidgets) {
            if (binder.binderWidgetGOs.TryGetValue(widgetKV.Key, out var widgetGO)) {
                UIWidget widget = widgetKV.Value;
                childWidgets.Add(widget);
                widget.onUnityResourceLoaded(widgetGO);
            }
        }
    }
}
