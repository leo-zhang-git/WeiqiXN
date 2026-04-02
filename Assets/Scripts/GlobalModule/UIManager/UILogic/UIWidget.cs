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

        foreach (var widget in binder.binderWidgets.Values) {
            childWidgets.Add(widget);
        }
    }

    public void LoadWidget(GameObject widgetGO = null)
    {
        if (widgetGO == null) {
            string assetPath = UIUtils.GetWidgetPrefabPath(widgetName);
            widgetGO = Global.Instance.resourceManager.LoadAsset<GameObject>(assetPath);
            if (widgetGO != null) {
                onUnityResourceLoaded(widgetGO);
            }
        } else {
            onUnityResourceLoaded(widgetGO);
        }
    }

    public void LoadWidgetAsync()
    {
        string assetPath = UIUtils.GetPagePrefabPath(widgetName);
        Global.Instance.resourceManager.LoadAssetAsync<GameObject>(this, assetPath, onUnityResourceLoaded);
    }
}
