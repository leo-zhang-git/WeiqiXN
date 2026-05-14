using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using XNLogger = XNClient.Logger.XNLogger;

public class ResourceManager : ModuleBase
{
    private const string ASSET_BUNDLE_MANIFEST_FILE_NAME = "bundle_manifest.json";

    public static uint ResourceBinderInstanceIds;
    public ResourceLoaderBase resLoader;
    public Dictionary<string, AssetBundle> bundleDict = new Dictionary<string, AssetBundle>(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> path2BundleName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public bool isReady { get; private set; }
    public bool isFailed { get; private set; }

    private Dictionary<string, IAssetRequest> requestMap = new Dictionary<string, IAssetRequest>();
    private Dictionary<string, IResourceLoadHandler> loadHandlerMap = new Dictionary<string, IResourceLoadHandler>();
    private Dictionary<string, IResourceLoadBinder> binderMap = new Dictionary<string, IResourceLoadBinder>();

#if UNITY_WEBGL && !UNITY_EDITOR
    private enum WebGLPreloadState
    {
        None,
        LoadingManifest,
        LoadingBundle,
        Done,
        Failed,
    }

    private WebGLPreloadState webGLPreloadState = WebGLPreloadState.None;
    private UnityWebRequest webGLManifestRequest;
    private UnityWebRequest webGLBundleRequest;
    private Queue<string> webGLPendingBundleNames = new Queue<string>();
    private string webGLCurrentBundleName;
#endif

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
        isReady = false;
        isFailed = false;

#if UNITY_EDITOR
        resLoader = new AssetDatabaseLoader(this);
        isReady = true;
#elif UNITY_WEBGL
        resLoader = new AssetBundleLoader(this);
        StartPreloadAssetBundlesWebGL();
#else
        PreloadAssetBundles();
        resLoader = new AssetBundleLoader(this);
        isReady = !isFailed;
#endif
    }

    public override void Update()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        UpdatePreloadAssetBundlesWebGL();
#endif

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
            isFailed = true;
            return;
        }

        foreach (string filePath in Directory.GetFiles(GlobalConfig.PATH_ASSET_BUNDLE)) {
            if (ShouldSkipAssetBundleFile(filePath)) {
                continue;
            }

            string bundleName = Path.GetFileName(filePath);
            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            if (bundle == null) {
                XNLogger.LogError("Load asset bundle failed.", ("bundlePath", filePath));
                continue;
            }

            RegisterLoadedAssetBundle(bundleName, bundle);
        }
    }

    private static bool ShouldSkipAssetBundleFile(string filePath)
    {
        string fileName = Path.GetFileName(filePath);
        string extension = Path.GetExtension(filePath);
        return string.Equals(fileName, ASSET_BUNDLE_MANIFEST_FILE_NAME, StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".manifest", StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".meta", StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase);
    }

    private void RegisterLoadedAssetBundle(string bundleName, AssetBundle bundle)
    {
        if (bundle == null) {
            XNLogger.LogError("Register null asset bundle failed.", ("bundleName", bundleName));
            return;
        }

        string canonicalBundleName = string.IsNullOrEmpty(bundle.name) ? bundleName : bundle.name;
        bundleDict[bundleName] = bundle;
        if (!string.Equals(bundleName, canonicalBundleName, StringComparison.OrdinalIgnoreCase)) {
            bundleDict[canonicalBundleName] = bundle;
        }

        foreach (string assetPath in bundle.GetAllAssetNames()) {
            path2BundleName[assetPath] = canonicalBundleName;
        }
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    private void StartPreloadAssetBundlesWebGL()
    {
        string manifestUrl = $"{GlobalConfig.PATH_ASSET_BUNDLE}/{ASSET_BUNDLE_MANIFEST_FILE_NAME}";
        webGLManifestRequest = UnityWebRequest.Get(manifestUrl);
        webGLManifestRequest.SendWebRequest();
        webGLPreloadState = WebGLPreloadState.LoadingManifest;
        XNLogger.LogInfo("Start preload WebGL asset bundle manifest.", ("manifestUrl", manifestUrl));
    }

    private void UpdatePreloadAssetBundlesWebGL()
    {
        switch (webGLPreloadState) {
            case WebGLPreloadState.LoadingManifest:
                UpdateWebGLManifestRequest();
                break;
            case WebGLPreloadState.LoadingBundle:
                UpdateWebGLBundleRequest();
                break;
        }
    }

    private void UpdateWebGLManifestRequest()
    {
        if (webGLManifestRequest == null || !webGLManifestRequest.isDone) {
            return;
        }

        if (webGLManifestRequest.result != UnityWebRequest.Result.Success) {
            XNLogger.LogError(
                "Load WebGL asset bundle manifest failed.",
                ("url", webGLManifestRequest.url),
                ("error", webGLManifestRequest.error)
            );
            FinishWebGLPreload(false);
            return;
        }

        try {
            JArray bundleNameArray = JArray.Parse(webGLManifestRequest.downloadHandler.text);
            foreach (JToken bundleNameToken in bundleNameArray) {
                string bundleName = bundleNameToken.Value<string>();
                if (!string.IsNullOrEmpty(bundleName)) {
                    webGLPendingBundleNames.Enqueue(bundleName);
                }
            }
        }
        catch (Exception ex) {
            XNLogger.LogError("Parse WebGL asset bundle manifest failed.", ("error", ex.Message));
            FinishWebGLPreload(false);
            return;
        }
        finally {
            webGLManifestRequest.Dispose();
            webGLManifestRequest = null;
        }

        if (webGLPendingBundleNames.Count <= 0) {
            XNLogger.LogWarn("WebGL asset bundle manifest is empty.");
            FinishWebGLPreload(true);
            return;
        }

        StartNextWebGLBundleRequest();
    }

    private void StartNextWebGLBundleRequest()
    {
        if (webGLPendingBundleNames.Count <= 0) {
            FinishWebGLPreload(true);
            return;
        }

        webGLCurrentBundleName = webGLPendingBundleNames.Dequeue();
        string bundleUrl = $"{GlobalConfig.PATH_ASSET_BUNDLE}/{webGLCurrentBundleName}";
        webGLBundleRequest = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl);
        webGLBundleRequest.SendWebRequest();
        webGLPreloadState = WebGLPreloadState.LoadingBundle;
        XNLogger.LogInfo("Start preload WebGL asset bundle.", ("bundleName", webGLCurrentBundleName), ("bundleUrl", bundleUrl));
    }

    private void UpdateWebGLBundleRequest()
    {
        if (webGLBundleRequest == null || !webGLBundleRequest.isDone) {
            return;
        }

        if (webGLBundleRequest.result != UnityWebRequest.Result.Success) {
            XNLogger.LogError(
                "Load WebGL asset bundle failed.",
                ("bundleName", webGLCurrentBundleName),
                ("url", webGLBundleRequest.url),
                ("error", webGLBundleRequest.error)
            );
            FinishWebGLPreload(false);
            return;
        }

        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webGLBundleRequest);
        if (bundle == null) {
            XNLogger.LogError("Downloaded WebGL asset bundle is null.", ("bundleName", webGLCurrentBundleName));
            FinishWebGLPreload(false);
            return;
        }

        RegisterLoadedAssetBundle(webGLCurrentBundleName, bundle);
        webGLBundleRequest.Dispose();
        webGLBundleRequest = null;
        webGLCurrentBundleName = string.Empty;
        StartNextWebGLBundleRequest();
    }

    private void FinishWebGLPreload(bool success)
    {
        webGLPreloadState = success ? WebGLPreloadState.Done : WebGLPreloadState.Failed;
        isReady = success;
        isFailed = !success;

        webGLManifestRequest?.Dispose();
        webGLManifestRequest = null;
        webGLBundleRequest?.Dispose();
        webGLBundleRequest = null;

        if (success) {
            XNLogger.LogInfo("Preload WebGL asset bundles success.", ("bundleCount", bundleDict.Count.ToString()));
        }
    }
#endif

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
        string assetFullPath = ResourceUtils.GetAssetFullPath<TAsset>(assetPath);
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

    public override void OnDestroy()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        webGLManifestRequest?.Dispose();
        webGLBundleRequest?.Dispose();
        webGLPendingBundleNames.Clear();
#endif
        HashSet<AssetBundle> unloadedBundles = new HashSet<AssetBundle>();
        foreach (AssetBundle bundle in bundleDict.Values) {
            if (bundle != null) {
                if (unloadedBundles.Contains(bundle)) {
                    continue;
                }

                bundle.Unload(false);
                unloadedBundles.Add(bundle);
            }
        }

        bundleDict.Clear();
        path2BundleName.Clear();
        requestMap.Clear();
        loadHandlerMap.Clear();
        binderMap.Clear();
        base.OnDestroy();
    }
}
