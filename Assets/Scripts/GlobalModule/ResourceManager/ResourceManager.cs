using System;
using System.Collections.Generic;

public class ResourceManager : ModuleBase
{
    public ResourceLoaderBase resLoader;
    private Dictionary<string, IAssetRequest> requestMap = new Dictionary<string, IAssetRequest>();
    private Dictionary<string, IResourceLoadHandler> loadHandlerMap = new Dictionary<string, IResourceLoadHandler>();
    private Dictionary<string, HashSet<string>> binderMap = new Dictionary<string, HashSet<string>>();

    public override void Init()
    {
#if UNITY_EDITOR
        resLoader = new AssetDatabaseLoader();
#else
        // TODO
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

        // ResourceLoadeHandler
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
            var loaderIdSet = binderKV.Value;
            List<string> pendingDeleteBinderLoader = new List<string>();
            foreach (var loaderId in loaderIdSet) {
                if (!loadHandlerMap.ContainsKey(loaderId)) {
                    pendingDeleteBinderLoader.Add(loaderId);
                }
            }
            foreach (var loaderId in pendingDeleteBinderLoader) {
                loaderIdSet.Remove(loaderId);
            }

            if (loaderIdSet.Count <= 0) {
                pendingDeleteBinder.Add(binderKV.Key);
            }
        }
        foreach (string binderId in pendingDeleteBinder) {
            binderMap.Remove(binderId);
        }
    }

    public T LoadAsset<T>(string path) where T : UnityEngine.Object
    {
        return resLoader.Loadasset<T>(path);
    }

    public IResourceLoadHandler LoadAssetAsync<TAsset>(IResourceLoadBinder binder, string assetPath, Action<TAsset> assetLoadedCB) where TAsset : UnityEngine.Object
    {
        string assetFullPath = ResourceUtils.GetAssetFullPath<TAsset>(assetPath);
        AssetRequest<TAsset> request;
        if (!requestMap.TryGetValue(assetFullPath, out var _request)) {
            request = resLoader.LoadAssetAsync<TAsset>(assetPath);
        } else {
            request = (AssetRequest<TAsset>)_request;
        }
        if (request == null) {
            return null;
        }

        IResourceLoadHandler loadHandler = new ResourceLoadHandler<TAsset>(binder.binderId, assetFullPath, assetLoadedCB);
        HashSet<string> loadHandlerIdSet;
        if (!binderMap.TryGetValue(binder.binderId, out loadHandlerIdSet)) {
            loadHandlerIdSet = new HashSet<string>();
            binderMap[binder.binderId] = loadHandlerIdSet;
        }
        loadHandlerIdSet.Add(loadHandler.loaderId);
        loadHandlerMap[loadHandler.loaderId] = loadHandler;

        requestMap[assetFullPath] = request;
        request.AddAssetLoadCB(loadHandler.OnAssetRequestLoaded);

        return loadHandler;
    }
}

