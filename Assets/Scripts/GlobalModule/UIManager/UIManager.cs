using System.Collections.Generic;

public class UIManager : ModuleBase
{
    private List<UIPage> mainPageStack = new List<UIPage>();

    public override void Init()
    {

    }

    public override void OnDestroy()
    {

    }

    public bool TryGetUIPage<TPage>(out TPage page) where TPage : UIPage
    {
        page = null;
        return false;
    }

    public void ShowMainPage<TPage>() where TPage : UIPage, new()
    {

    }

    public void ShowPopupPage<TPage>() where TPage : UIPage, new()
    {
        if (mainPageStack.Count <= 0) {
            Logger.LogError("Main page stack is empty, show popup failed", ("pageName", typeof(TPage).Name));
            return;
        }
        UIPage topPage = mainPageStack[mainPageStack.Count - 1];
        TPage popupPage = new TPage();
        topPage.AddPopupPage(popupPage);
    }

    public void ClosePage<TPage>() where TPage : UIPage, new()
    {

    }
}