using System;
using System.Collections.Generic;

public interface IAssetRequest
{
    public string assetFullPath { get; }
    public bool isLoaded { get; }
    public UnityEngine.Object asset { get; }
    public void Update();
}

public abstract class AssetRequest<TAsset> : IAssetRequest where TAsset : UnityEngine.Object
{
    public abstract bool isLoaded { get; }
    public TAsset _asset;
    public UnityEngine.Object asset => _asset;
    private string _assetFulltPath;
    public string assetFullPath => _assetFulltPath;
    public List<Action<IAssetRequest>> assetLoadedCBs = new List<Action<IAssetRequest>>();
    protected int refCount = 0;

    public AssetRequest(string asseFulltPath)
    {
        _assetFulltPath = asseFulltPath;
    }

    protected abstract bool CheckLoadDone();

    public void Update()
    {
        if (refCount <= 0) {
            Dispose();
            return;
        }

        if (CheckLoadDone()) {
            foreach (var loadedCB in assetLoadedCBs) {
                loadedCB.Invoke(this);
                refCount -= 1;
            }
        }
    }

    public void AddAssetLoadCB(Action<IAssetRequest> loadedCB)
    {
        assetLoadedCBs.Add(loadedCB);
        refCount += 1;
    }

    public virtual void Dispose()
    {
        _assetFulltPath = string.Empty;
        refCount = 0;
        _asset = null;
        assetLoadedCBs.Clear();
    }
}
