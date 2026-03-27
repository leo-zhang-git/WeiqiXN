using System.Collections.Generic;

public class UIManager : ModuleBase
{
    public UIContext topContext;
    public List<UIContext> contextStack = new List<UIContext>();
    public int topCanvasOrder;

    public override void Init()
    {
        topCanvasOrder = GlobalConfig.BASE_CANVAS_ORDER;
    }

    public override void OnDestroy()
    {

    }

    public void ShowMainPage<TPage>() where TPage : UIPage, new()
    {
        TPage page = new TPage();
        page.pageFlags = UIPage.PageFlags.MainPage;
        topCanvasOrder += GlobalConfig.CONTEXT_INCREASE_CANVAS_ORDER;
        UIContext context = new UIContext(topCanvasOrder);
        context.SetMainPage(page);
    }

    public void ShowPopupPage<TPage>() where TPage : UIPage, new()
    {
        TPage page = new TPage();
        page.pageFlags = UIPage.PageFlags.PopupPage;
        if (topContext == null) {
            topCanvasOrder += GlobalConfig.CONTEXT_INCREASE_CANVAS_ORDER;
            topContext = new UIContext(topCanvasOrder);
        }
        topContext.AddPopupPage(page);
    }

    public void ClosePage<TPage>() where TPage : UIPage
    {

    }
}