public abstract class UIWidget : UILogicBase
{
    public void CloseWidget()
    {

    }
}

public abstract class UIWidgetWithBinder<TBinder> : UIWidget where TBinder : UIBinderBase
{
    public TBinder binder;

    protected override void OnLoaded()
    {
        base.OnLoaded();
        binder = gameObject.GetComponent<TBinder>();
        binder.InitWidgets();
    }
}
