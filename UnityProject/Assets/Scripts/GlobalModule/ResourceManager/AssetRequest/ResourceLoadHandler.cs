using System;
using System.IO;
using XNClient.Logger;

public interface IResourceLoadHandler
{
    public string binderId { get; }
    public string loaderId { get; }
    public bool isLoading { get; }
    public bool isCanceled { get; }
    public void Cancel();
    public void OnAssetRequestLoaded(IAssetRequest assetRequest);
}

public class ResourceLoadHandler<TAsset> : IResourceLoadHandler where TAsset : UnityEngine.Object
{
    public static uint LoadHandlerIds;
    public string _binderId;
    public string binderId => _binderId;
    public string assetFullPath;
    public bool isCanceled => loadedCB == null;
    private string _loaderId;
    public string loaderId => _loaderId;
    public bool isLoading
    {
        get
        {
            return false;
        }
    }
    public Action<TAsset> loadedCB;

    public ResourceLoadHandler(string binderId, string assetFullPath, Action<TAsset> loadedCB)
    {
        this._binderId = binderId;
        this.assetFullPath = assetFullPath;
        string fileName = Path.GetFileName(assetFullPath);
        this._loaderId = $"{binderId}_{fileName}_{LoadHandlerIds}";
        LoadHandlerIds += 1;
        this.loadedCB = loadedCB;
    }

    public void OnAssetRequestLoaded(IAssetRequest assetRequest)
    {
        if (loadedCB != null) {
            try {
                loadedCB.Invoke(assetRequest.asset as TAsset);
            }
            catch (Exception e) {
                XNLogger.LogError("Resource load handler loaded callback error", ("errMsg", e.Message), ("assetFullPath", assetFullPath));
            }
        }

        Dispose();
    }

    public void Cancel()
    {
        loadedCB = null;
        XNLogger.LogInfo("Resource load handler canceled.", ("loaderId", loaderId));
    }

    public void Dispose()
    {
        _binderId = string.Empty;
        _loaderId = string.Empty;
        assetFullPath = string.Empty;
        loadedCB = null;
    }
}

