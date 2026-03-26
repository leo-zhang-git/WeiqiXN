using System.Collections.Generic;

public class UIManager : BaseModule
{
    private Stack<UIPage> mainPageStack = new Stack<UIPage>();

    public override void Init()
    {

    }

    public override void OnDestroy()
    {

    }

    public void ShowMainPage<TPage>() where TPage : UIPage
    {

    }

    public void ShowPopupPage<TPage>() where TPage : UIPage
    {

    }
}