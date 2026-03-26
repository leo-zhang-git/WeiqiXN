public abstract class UIWidget : UILogicBase
{

}

public abstract class UIWidgetWithBinder<TBinder> : UIWidget where TBinder : UILogicBinder
{
    public TBinder binder;
}
