using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : ModuleBase
{
    public Dictionary<UIContextType, UIContext> contextDict = new Dictionary<UIContextType, UIContext>();
    public GameObject uiRoot;

    public override void Init()
    {
        uiRoot = new GameObject(UIConfig.NAME_UI_ROOT);
        GameObject.DontDestroyOnLoad(uiRoot);

        foreach (UIContextType type in Enum.GetValues(typeof(UIContextType))) {
            contextDict.TryAdd(type, new UIContext(type));
        }
    }

    public void ShowMainPage<TPage>(UIContextType contextType) where TPage : UIPage, new()
    {
        if (contextDict.TryGetValue(contextType, out var uiContext)) {
            TPage page = new TPage();
            page.pageFlags = UIPageFlags.MainPage;
            uiContext.ShowMainPage(page);
        } else {
            Logger.LogError("Invalid context type for show main page", ("contextType", contextType.ToString()));
        }
    }

    public void ShowPopupPage<TPage>(UIContextType contextType) where TPage : UIPage, new()
    {
        if (contextDict.TryGetValue(contextType, out var uiContext)) {
            TPage page = new TPage();
            page.pageFlags = UIPageFlags.PopupPage;
            uiContext.ShowPopupPage(page);
        } else {
            Logger.LogError("Invalid context type for show popup page", ("contextType", contextType.ToString()));
        }
    }

    public void CloseMainPage<TPage>(UIContextType contextType, TPage page) where TPage : UIPage
    {
        if (contextDict.TryGetValue(contextType, out var uiContext)) {
            uiContext.CloseMainPage(page);
        } else {
            Logger.LogError("Invalid context type for close main page", ("contextType", contextType.ToString()));
        }
    }

    public void ClosePopupPage<TPage>(UIContextType contextType, TPage page) where TPage : UIPage
    {
        if (contextDict.TryGetValue(contextType, out var uiContext)) {
            uiContext.ClosePopupPage(page);
        } else {
            Logger.LogError("Invalid context type for close popup page", ("contextType", contextType.ToString()));
        }
    }
}