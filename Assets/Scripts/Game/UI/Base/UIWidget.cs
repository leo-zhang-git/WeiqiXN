public abstract class UIWidget : UILogicBase
{
    public void CloseWidget()
    {

    }
}

public abstract class UIWidgetWithBinder<TBinder> : UIWidget where TBinder : UILogicBinder
{
    public TBinder binder;
}
