using System.Collections.Generic;

public abstract class UIPage : UILogicBase
{
    protected List<UIWidget> childWidgets = new List<UIWidget>();
}

public abstract class UIPageWithBinder<TBinder> : UIPage where TBinder : UILogicBinder
{
    public TBinder binder;

    protected override void OnLoaded()
    {
        base.OnLoaded();
    }
}
