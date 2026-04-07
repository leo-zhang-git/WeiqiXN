using System.Collections.Generic;
using UnityEngine;

public abstract class UIWidget : UILogicBase
{
    public readonly UILogicBase owner;
    private string _widgetName;
    public string widgetName
    {
        get
        {
            if (_widgetName == null) {
                _widgetName = GetType().Name;
            }
            return _widgetName;
        }
    }
    protected List<UIWidget> childWidgets = new List<UIWidget>();

    public UIWidget(UILogicBase owner)
    {
        this.owner = owner;
    }

    public void CloseWidget()
    {
        foreach (var widget in childWidgets) {
            widget.CloseWidget();
        }
        OnHide();
        OnClose();
    }
}

public abstract class UIWidgetWithBinder<TBinder> : UIWidget where TBinder : UIBinderBase
{
    public TBinder binder;

    protected UIWidgetWithBinder(UILogicBase owner) : base(owner)
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

    public override void SetUIVisible(bool isVisible)
    {
        base.SetUIVisible(isVisible);

        foreach (var widget in childWidgets) {
            widget.SetUIVisible(isVisible);
        }
    }

    public void LoadWidget(bool isAsync = true)
    {
        string assetPath = UIUtils.GetPagePrefabPath(widgetName);
        if (isAsync) {
            Global.Instance.resourceManager.LoadAssetAsync<GameObject>(this, assetPath, onUnityResourceLoaded);
        } else {
            GameObject widgetGO = Global.Instance.resourceManager.LoadGamePrefab(assetPath);
            if (widgetGO != null) {
                onUnityResourceLoaded(widgetGO);
            }
        }
    }
}
