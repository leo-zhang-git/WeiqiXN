using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : ModuleBase
{
    public GameObject uiRoot;
    public GameObject uiEventSystemGO;
    private Dictionary<UIContextType, UIContext> contextDict = new Dictionary<UIContextType, UIContext>();
    private class CachePageInfo
    {
        public float cacheDuration; // 已经缓存的时间
        public GameObject cacheGO;

        public CachePageInfo(GameObject cacheGO)
        {
            cacheDuration = 0;
            this.cacheGO = cacheGO;
        }
    }
    private Dictionary<string, CachePageInfo> cachePages = new Dictionary<string, CachePageInfo>();

    public override void Init()
    {
        uiRoot = new GameObject(UIConfig.NAME_UI_ROOT);
        GameObject.DontDestroyOnLoad(uiRoot);
        uiEventSystemGO = Global.Instance.resourceManager.LoadGamePrefabWithConfigId(UIConfig.UI_EVENTSYSTEM_CONFIG_ID);
        if (uiEventSystemGO != null) {
            GameObject.DontDestroyOnLoad(uiEventSystemGO);
        } else {
            Logger.LogError("UI event system go create failed!!!!!");
        }

        foreach (UIContextType type in Enum.GetValues(typeof(UIContextType))) {
            contextDict.TryAdd(type, new UIContext(type));
        }
    }

    public override void Update()
    {
        base.Update();

        List<string> pendingDeleteCachePage = new List<string>();
        foreach (var cachePageKV in cachePages) {
            cachePageKV.Value.cacheDuration += Time.deltaTime;
            if (cachePageKV.Value.cacheDuration >= UIConfig.PAGE_GAMEOBJECT_CACHE_TIME) {
                GameObject.Destroy(cachePageKV.Value.cacheGO);
                pendingDeleteCachePage.Add(cachePageKV.Key);
            }
        }
        foreach (string pageName in pendingDeleteCachePage) {
            cachePages.Remove(pageName);
        }
    }

    public void ShowMainPage<TPage>() where TPage : UIPage, new()
    {
        UiPageDataType uiConfig = UiPageDataType.GetConfigData(UIPage.GetPageName<TPage>());
        if (uiConfig == null) {
            Logger.LogError("Invalid ui config, show main page failed.", ("pageName", UIPage.GetPageName<TPage>()));
            return;
        }
        UIContextType contextType = UIUtils.ParseUIContextType(uiConfig.contextType);

        if (contextDict.TryGetValue(contextType, out var uiContext)) {
            TPage page = UIPage.CreatePageInstance<TPage>(uiContext, UIPageFlags.MainPage);
            uiContext.ShowMainPage(page);
        } else {
            Logger.LogWarn("Invalid context type for show main page", ("contextType", contextType.ToString()));
        }
    }

    public void ShowPopupPage<TPage>() where TPage : UIPage, new()
    {
        UiPageDataType uiConfig = UiPageDataType.GetConfigData(UIPage.GetPageName<TPage>());
        if (uiConfig == null) {
            Logger.LogError("Invalid ui config, show popup page failed.", ("pageName", UIPage.GetPageName<TPage>()));
            return;
        }
        UIContextType contextType = UIUtils.ParseUIContextType(uiConfig.contextType);

        if (contextDict.TryGetValue(contextType, out var uiContext)) {
            TPage page = UIPage.CreatePageInstance<TPage>(uiContext, UIPageFlags.PopupPage);
            uiContext.ShowPopupPage(page);
        } else {
            Logger.LogWarn("Invalid context type for show popup page", ("contextType", contextType.ToString()));
        }
    }

    public void CloseMainPage(UIPage page)
    {
        if (page.owner.CloseMainPage(page)) {
            RecycleClosedPage(page);
        }
    }

    public void CloseMainPage<TPage>() where TPage : UIPage
    {
        UiPageDataType uiConfig = UiPageDataType.GetConfigData(UIPage.GetPageName<TPage>());
        if (uiConfig == null) {
            Logger.LogError("Invalid ui config, close main page failed.", ("pageName", UIPage.GetPageName<TPage>()));
            return;
        }
        UIContextType contextType = UIUtils.ParseUIContextType(uiConfig.contextType);

        UIContext uiContext;
        if (contextDict.TryGetValue(contextType, out uiContext)) {
            TPage page = uiContext.GetMainPage<TPage>();
            if (page != null) {
                CloseMainPage(page);
            } else {
                Logger.LogWarn("Page not found, close main page failed.", ("pageName", UIPage.GetPageName<TPage>()), ("contextType", contextType.ToString()));
            }
        }
    }

    public void ClosePopupPage(UIPage page)
    {
        if (page.owner.ClosePopupPage(page)) {
            RecycleClosedPage(page);
        }
    }

    public void ClosePopupPage<TPage>() where TPage : UIPage
    {
        UiPageDataType uiConfig = UiPageDataType.GetConfigData(UIPage.GetPageName<TPage>());
        if (uiConfig == null) {
            Logger.LogError("Invalid ui config, close popup page failed.", ("pageName", UIPage.GetPageName<TPage>()));
            return;
        }
        UIContextType contextType = UIUtils.ParseUIContextType(uiConfig.contextType);

        UIContext uiContext;
        if (contextDict.TryGetValue(contextType, out uiContext)) {
            TPage page = uiContext.GetPopupPage<TPage>();
            if (page != null) {
                ClosePopupPage(page);
            } else {
                Logger.LogWarn("Page not found, close popup page failed.", ("pageName", UIPage.GetPageName<TPage>()), ("contextType", contextType.ToString()));
            }
        }
    }

    private void RecycleClosedPage(UIPage page)
    {
        if (page.gameObject == null) {
            return;
        }

        if (cachePages.TryGetValue(page.pageName, out var pageInfo)) {
            pageInfo.cacheDuration = 0;
            GameObject.Destroy(page.gameObject);
        } else {
            page.gameObject.SetActive(false);
            cachePages[page.pageName] = new CachePageInfo(page.gameObject);
        }
    }

    public bool TryGetCachePageGO(string pageName, out GameObject pageGO)
    {
        pageGO = null;
        if (cachePages.TryGetValue(pageName, out var pageInfo)) {
            pageGO = pageInfo.cacheGO;
            cachePages.Remove(pageName);
            return true;
        } else {
            return false;
        }
    }
}