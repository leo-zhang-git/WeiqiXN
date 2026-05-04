using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XNLogger = XNClient.Logger.XNLogger;

public class ResourceManager : ModuleBase
{
    public static uint ResourceBinderInstanceIds;
    public ResourceLoaderBase resLoader;
    public Dictionary<string, AssetBundle> bundleDict = new Dictionary<string, AssetBundle>();
    public Dictionary<string, string> path2BundleName = new Dictionary<string, string>();

    private Dictionary<string, IAssetRequest> requestMap = new Dictionary<string, IAssetRequest>();
    private Dictionary<string, IResourceLoadHandler> loadHandlerMap = new Dictionary<string, IResourceLoadHandler>();
    private Dictionary<string, IResourceLoadBinder> binderMap = new Dictionary<string, IResourceLoadBinder>();

    protected class PackInfoFile
    {
        [JsonProperty("bundles")] public Dictionary<string, BundleInfo> Bundles { get; set; }
    }

    protected class BundleInfo
    {
        [JsonProperty("hash")] public string Hash { get; set; }
        [JsonProperty("size")] public int Size { get; set; }
    }

    public override void Init()
    {
        PreloadAssetBundles();
#if UNITY_EDITOR
        resLoader = new AssetDatabaseLoader(this);
#else
        resLoader = new AssetBundleLoader(this);
#endif
    }

    public override void Update()
    {
        // AssetRequest
        List<string> pendingDeleteRequest = new List<string>();
        foreach (var requestKV in requestMap) {
            if (requestKV.Value.isLoaded) {
                pendingDeleteRequest.Add(requestKV.Key);
                continue;
            }
            requestKV.Value.Update();
        }
        foreach (string assetFullPath in pendingDeleteRequest) {
            requestMap.Remove(assetFullPath);
        }

        // ResourceLoadHandler
        List<string> pendingDeleteHandler = new List<string>();
        foreach (var handlerKV in loadHandlerMap) {
            if (handlerKV.Value.isCanceled) {
                pendingDeleteHandler.Add(handlerKV.Key);
            }
        }
        foreach (string loaderId in pendingDeleteHandler) {
            loadHandlerMap.Remove(loaderId);
        }

        // ResourceLoadBinder
        List<string> pendingDeleteBinder = new List<string>();
        foreach (var binderKV in binderMap) {
            var loadHandlerIds = binderKV.Value.loadHandlerIds;
            List<string> pendingDeleteBinderLoader = new List<string>();
            foreach (var loaderId in loadHandlerIds) {
                if (!loadHandlerMap.ContainsKey(loaderId)) {
                    pendingDeleteBinderLoader.Add(loaderId);
                }
            }
            foreach (var loaderId in pendingDeleteBinderLoader) {
                loadHandlerIds.Remove(loaderId);
            }

            if (loadHandlerIds.Count <= 0) {
                pendingDeleteBinder.Add(binderKV.Key);
            }
        }
        foreach (string binderId in pendingDeleteBinder) {
            binderMap.Remove(binderId);
        }
    }

    public void PreloadAssetBundles()
    {
        if (!Directory.Exists(GlobalConfig.PATH_ASSET_BUNDLE)) {
            XNLogger.LogError("Asset bundle directory not found.", ("bundleDir", GlobalConfig.PATH_ASSET_BUNDLE));
            return;
        }

        foreach (string filePath in Directory.GetFiles(GlobalConfig.PATH_ASSET_BUNDLE)) {
            string bundleName = Path.GetFileName(filePath);
            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            if (bundle == null) {
                XNLogger.LogError("Load asset bundle failed.", ("bundlePath", filePath));
                continue;
            }

            bundleDict[bundleName] = bundle;
            foreach (string assetPath in bundle.GetAllAssetNames()) {
                path2BundleName[assetPath] = bundleName;
            }
        }
    }

    public GameObject LoadGamePrefabWithConfigId(string configId)
    {
        var config = GamePrefabDataType.GetConfigData(configId);
        if (config != null) {
            return LoadGamePrefab(config.resPath);
        } else {
            XNLogger.LogError("Config id invalid, laod game prefab failed.", ("configId", configId));
            return null;
        }
    }

    public GameObject LoadGamePrefab(string assetPath)
    {
        GameObject asset = LoadAsset<GameObject>(assetPath);
        if (asset != null) {
            var go = GameObject.Instantiate(asset);
            return go;
        }

        return null;
    }

    public IResourceLoadHandler LoadGamePrefabAsyncWithConfigId(IResourceLoadBinder binder, string configId, Action<GameObject> goInstantiateCB)
    {
        var config = GamePrefabDataType.GetConfigData(configId);
        if (config != null) {
            return LoadGamePrefabAsync(binder, config.resPath, goInstantiateCB);
        } else {
            XNLogger.LogError("Config id invalid, load game prefab async failed.", ("configId", configId));
            return null;
        }
    }

    public IResourceLoadHandler LoadGamePrefabAsync(IResourceLoadBinder binder, string assetPath, Action<GameObject> goInstantiateCB)
    {
        Action<GameObject> assetLoadedCB = (GameObject asset) =>
        {
            GameObject go = GameObject.Instantiate(asset);
            goInstantiateCB.Invoke(go);
        };
        var loadHandler = LoadAssetAsync<GameObject>(binder, assetPath, assetLoadedCB);
        if (loadHandler != null) {
            return loadHandler;
        }

        return null;
    }

    public TAsset LoadAsset<TAsset>(string assetPath) where TAsset : UnityEngine.Object
    {
        string assetFullPath = ResourceUtils.GetAssetFullPath<GameObject>(assetPath);
        if (string.IsNullOrEmpty(assetFullPath)) {
            return null;
        }
        return resLoader.Loadasset<TAsset>(assetFullPath);
    }

    public IResourceLoadHandler LoadAssetAsync<TAsset>(IResourceLoadBinder binder, string assetPath, Action<TAsset> assetLoadedCB) where TAsset : UnityEngine.Object
    {
        string assetFullPath = ResourceUtils.GetAssetFullPath<TAsset>(assetPath);
        if (string.IsNullOrEmpty(assetFullPath)) {
            return null;
        }
        AssetRequest<TAsset> request;
        if (!requestMap.TryGetValue(assetFullPath, out var _request)) {
            request = resLoader.LoadAssetAsync<TAsset>(assetFullPath);
        } else {
            request = (AssetRequest<TAsset>)_request;
        }
        if (request == null) {
            return null;
        }

        IResourceLoadHandler loadHandler = new ResourceLoadHandler<TAsset>(binder.binderId, assetFullPath, assetLoadedCB);
        if (!binderMap.ContainsKey(binder.binderId)) {
            binderMap[binder.binderId] = binder;
        }
        binder.loadHandlerIds.Add(loadHandler.loaderId);
        loadHandlerMap[loadHandler.loaderId] = loadHandler;

        requestMap[assetFullPath] = request;
        request.AddAssetLoadCB(loadHandler.OnAssetRequestLoaded);

        return loadHandler;
    }

    public void OnResourceBinderDestroyed(string binderId)
    {
        if (binderMap.TryGetValue(binderId, out var binder)) {
            foreach (string loaderId in binder.loadHandlerIds) {
                if (loadHandlerMap.TryGetValue(loaderId, out var loader)) {
                    loader.Cancel();
                }
            }
            binderMap.Remove(binderId);
        }
    }
}

