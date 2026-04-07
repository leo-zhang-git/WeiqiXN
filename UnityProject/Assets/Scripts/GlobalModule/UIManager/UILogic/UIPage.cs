using System.Collections.Generic;
using UnityEngine;

public abstract class UIPage : UILogicBase
{
    public UIContext owner;
    public UIContext pageContext;
    public int canvasOrder;

    public UIPageFlags pageFlags;
    public bool isMainPage => (pageFlags & UIPageFlags.MainPage) == UIPageFlags.MainPage;
    public bool isPopupPage => (pageFlags & UIPageFlags.PopupPage) == UIPageFlags.PopupPage;

    private string _pageName;
    public string pageName
    {
        get
        {
            if (_pageName == null) {
                _pageName = GetType().Name;
            }
            return _pageName;
        }
    }
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

    public UIPage(UIContext owner)
    {
        this.owner = owner;
    }

    protected override void OnLoaded()
    {
        base.OnLoaded();
        canvas.sortingOrder = canvasOrder;
        OnOpen();

        if (isMainPage) {
            if (owner.mainPageStack.Last.Value == this) {
                var previousPage = owner.mainPageStack.Last.Previous.Value;
                previousPage.AddResourceLoadedCB(() =>
                {
                    previousPage.SetUIVisible(false);
                });
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

    public void LoadPage(bool isAsync = true)
    {
        if (Global.Instance.uiManager.TryGetCachePageGO(pageName, out GameObject pageGO)) {
            pageGO.SetActive(true);
            onUnityResourceLoaded(pageGO);
        } else {
            string assetPath = UIUtils.GetPagePrefabPath(pageName);
            if (isAsync) {
                Global.Instance.resourceManager.LoadAssetAsync<GameObject>(this, assetPath, onUnityResourceLoaded);
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
        Global.Instance.uiManager.RecycleClosedPage(this);
        OnClose();
    }
}

public abstract class UIPageWithBinder<TBinder> : UIPage where TBinder : UIBinderBase
{
    public TBinder binder;

    protected UIPageWithBinder(UIContext owner) : base(owner)
    {

    }

    protected override void OnLoaded()
    {
        base.OnLoaded();
        binder = gameObject.GetComponent<TBinder>();
        binder.InitWidgets(this);

        foreach (var widgetKV in binder.binderWidgets) {
            if (binder.binderWidgetGOs.TryGetValue(widgetKV.Key, out var widgetGO)) {
                var widget = widgetKV.Value;
                childWidgets.Add(widget);
                widget.onUnityResourceLoaded(widgetGO);
            }
        }
    }
}
