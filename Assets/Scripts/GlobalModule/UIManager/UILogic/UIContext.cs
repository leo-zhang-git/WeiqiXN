using System.Collections.Generic;

public class UIContext
{
    public readonly UIContextType contextType;
    public LinkedList<UIPage> mainPageStack = new LinkedList<UIPage>();
    public List<UIPage> popupList = new List<UIPage>();
    public int baseCanvasOrder;

    public UIContext(UIContextType contextType)
    {
        this.contextType = contextType;
        baseCanvasOrder = UIUtils.GetUIContextBaseOrder(contextType);
    }

    public void OnDestroy()
    {

    }

    public void ShowMainPage(UIPage mainPage)
    {
        mainPage.canvasOrder = baseCanvasOrder + mainPageStack.Count * UIConfig.MAINPAGE_INCREASE_CANVAS_ORDER;
        mainPage.LoadPageAsync();
        mainPageStack.AddLast(mainPage);
    }

    public void CloseMainPage(UIPage mainPage)
    {
        if (mainPageStack.Contains(mainPage)) {
            if (mainPageStack.Last.Value != mainPage) {
                Logger.LogError("Try to close main page not on stack top", ("pageName", mainPage.pageName), ("contextType", contextType.ToString()));
                mainPageStack.Remove(mainPage);
            } else {
                mainPageStack.RemoveLast();
            }
            CloseAllPopupPages();
            mainPage.ClosePage();

            if (mainPageStack.Count > 0) {
                mainPageStack.Last.Value.SetUIVisible(true);
            }
        } else {
            Logger.LogError("Target main page not in current context", ("pageName", mainPage.pageName), ("contextType", contextType.ToString()));
        }
    }

    public void ShowPopupPage(UIPage popupPage)
    {
        popupPage.canvasOrder = baseCanvasOrder + popupList.Count * UIConfig.POPUP_INCREASE_CANVAS_ORDER;
        popupPage.LoadPageAsync();
        popupList.Add(popupPage);
    }

    public void ClosePopupPage(UIPage popupPage)
    {
        if (popupList.Contains(popupPage)) {
            popupList.Remove(popupPage);
            for (int i = 0; i < popupList.Count; i++) {
                popupList[i].canvasOrder = baseCanvasOrder + (i + 1) * UIConfig.POPUP_INCREASE_CANVAS_ORDER;
            }
        } else {
            Logger.LogError("Target popup page not in current context", ("pageName", popupPage.pageName), ("contextType", contextType.ToString()));
        }
    }

    public void CloseAllPopupPages()
    {
        foreach (var page in popupList) {
            page.ClosePage();
        }
        popupList.Clear();
    }
}
