using System;
using UnityEditor;

public class AssetDatabaseLoader : ResourceLoaderBase
{
    public override bool CheckAssetExisits(string assetFullPath)
    {
        string assetGuid = AssetDatabase.AssetPathToGUID(assetFullPath);
        return !string.IsNullOrEmpty(assetGuid);
    }

    public override T Loadasset<T>(string assetFullPath)
    {
        if (!CheckAssetExisits(assetFullPath)) {
            Logger.LogError("Target asset not exists!", ("assetFullPath", assetFullPath));
            return null;
        }

        try {
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetFullPath);
            return asset;
        }
        catch (Exception e) {
            Logger.LogError("Resource load by asset database failed.", ("assetFullPath", assetFullPath), ("exception", e.Message));
        }
        return null;
    }

    public override AssetRequest<T> LoadAssetAsync<T>(string assetFullPath)
    {
        T asset = Loadasset<T>(assetFullPath);
        if (asset) {
            SimulateLoadAsyncRequest<T> request = new SimulateLoadAsyncRequest<T>(assetFullPath, asset);
            return request;
        }
        return null;
    }
}
