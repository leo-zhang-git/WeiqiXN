using System.Collections.Generic;

public class UIContext
{
    public UIPage mainPage;
    public List<UIPage> popupList = new List<UIPage>();
    public int baseCanvasOrder;

    public UIContext(int baseCanvasOrder)
    {
        this.baseCanvasOrder = baseCanvasOrder;
    }

    public void OnDestroy()
    {

    }

    public void SetMainPage(UIPage mainPage)
    {
        if (this.mainPage == null) {
            this.mainPage = mainPage;
            this.mainPage.canvasOrder = baseCanvasOrder;
            this.mainPage.LoadPage();
        } else {
            Logger.LogError("Duplicated mainPage for UIContext.");
        }
    }

    public void AddPopupPage(UIPage popupPage)
    {
        popupList.Add(popupPage);
        popupPage.canvasOrder = baseCanvasOrder + popupList.Count * GlobalConfig.POPUP_INCREASE_CANVAS_ORDER;
        popupPage.LoadPage();
    }

    public void RemovePopupPage(UIPage popupPage)
    {
        popupList.Remove(popupPage);
    }
}
