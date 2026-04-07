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
        GamePrefabDataType uiEventSystemCfg = GamePrefabDataType.GetConfigData(UIConfig.UI_EVENTSYSTEM_CONFIG_ID);
        if (uiEventSystemCfg != null) {
            // TODO
        } else {
            Logger.LogError("UI event system creation failed!");
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

    public void RecycleClosedPage(UIPage page)
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