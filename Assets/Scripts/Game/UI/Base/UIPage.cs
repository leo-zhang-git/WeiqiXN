using System.Collections.Generic;

public abstract class UIPage : UILogicBase
{
    protected List<UIWidget> childWidgets = new List<UIWidget>();
    protected List<UIPage> popupList = new List<UIPage>();

    protected override void OnClose()
    {
        base.OnClose();

        foreach (var popupPage in popupList) {
            popupPage.ClosePage();
        }
        foreach (var widget in childWidgets) {
            widget.CloseWidget();
        }
    }

    public void AddPopupPage(UIPage popupPage)
    {
        popupList.Add(popupPage);
    }

    public void RemovePopupPage(UIPage popupPage)
    {
        popupList.Remove(popupPage);
    }

    public void ClosePage()
    {

    }
}

public abstract class UIPageWithBinder<TBinder> : UIPage where TBinder : UILogicBinder
{
    public TBinder binder;

    protected override void OnLoaded()
    {
        base.OnLoaded();
    }
}
