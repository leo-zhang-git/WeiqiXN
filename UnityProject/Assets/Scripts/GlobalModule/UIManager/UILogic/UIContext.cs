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

    public TPage GetMainPage<TPage>() where TPage : UIPage
    {
        TPage page = null;
        foreach (var _page in mainPageStack) {
            if (_page.pageName == UIPage.GetPageName<TPage>()) {
                page = (TPage)_page;
                break;
            }
        }

        return page;
    }

    public TPage GetPopupPage<TPage>() where TPage : UIPage
    {
        TPage page = null;
        foreach (var _page in popupList) {
            if (_page.pageName == UIPage.GetPageName<TPage>()) {
                page = (TPage)_page;
                break;
            }
        }

        return page;
    }

    public void ShowMainPage(UIPage mainPage)
    {
        mainPage.canvasOrder = baseCanvasOrder + mainPageStack.Count * UIConfig.MAINPAGE_INCREASE_CANVAS_ORDER;
        mainPage.LoadPage();
        mainPageStack.AddLast(mainPage);
        Logger.LogInfo("UIContext show main page.", ("contextType", contextType.ToString()), ("pageName", mainPage.pageName));
    }

    public bool CloseMainPage(UIPage mainPage)
    {
        if (mainPageStack.Contains(mainPage)) {
            if (mainPageStack.Count > 0) {
                if (mainPageStack.Last.Value != mainPage) {
                    Logger.LogWarn("Try to close main page not on stack top", ("pageName", mainPage.pageName), ("contextType", contextType.ToString()));
                    mainPageStack.Remove(mainPage);
                } else {
                    mainPageStack.RemoveLast();
                }
            }
            CloseAllPopupPages();

            if (mainPageStack.Count > 0) {
                mainPageStack.Last.Value.SetUIVisible(true);
            }
            Logger.LogInfo("UIContext close main page.", ("contextType", contextType.ToString()), ("pageName", mainPage.pageName));
            return true;
        } else {
            Logger.LogError("Target main page not in current context", ("pageName", mainPage.pageName), ("contextType", contextType.ToString()));
            return false;
        }
    }

    public void ShowPopupPage(UIPage popupPage)
    {
        popupPage.canvasOrder =
            baseCanvasOrder + mainPageStack.Count * UIConfig.MAINPAGE_INCREASE_CANVAS_ORDER +
            popupList.Count * UIConfig.POPUP_INCREASE_CANVAS_ORDER;
        popupPage.LoadPage();
        popupList.Add(popupPage);
        Logger.LogInfo("UIContext show popup page.", ("contextType", contextType.ToString()), ("pageName", popupPage.pageName));
    }

    public bool ClosePopupPage(UIPage popupPage)
    {
        if (popupList.Contains(popupPage)) {
            popupList.Remove(popupPage);
            for (int i = 0; i < popupList.Count; i++) {
                popupList[i].canvasOrder = baseCanvasOrder + (i + 1) * UIConfig.POPUP_INCREASE_CANVAS_ORDER;
            }
            Logger.LogInfo("UIContext close popup page.", ("contextType", contextType.ToString()), ("pageName", popupPage.pageName));
            return true;
        } else {
            Logger.LogError("Target popup page not in current context", ("pageName", popupPage.pageName), ("contextType", contextType.ToString()));
            return false;
        }
    }

    public void CloseAllPopupPages()
    {
        foreach (var page in popupList) {
            page.ClosePage();
        }
        popupList.Clear();
        Logger.LogInfo("UIContext close all popup pages.", ("contextType", contextType.ToString()));
    }
}
