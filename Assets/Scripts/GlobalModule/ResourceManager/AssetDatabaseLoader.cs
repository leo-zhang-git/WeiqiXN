using System;
using UnityEditor;

public class AssetDatabaseLoader : ResourceLoaderBase
{
    public override T Loadasset<T>(string assetName)
    {
        string assetPath = GetAssetFullPath<T>(assetName);
        if (!string.IsNullOrEmpty(assetPath)) {
            try {
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
            catch (Exception e) {
                Logger.LogError("Resource load by asset database failed.", ("assetName", assetName), ("assetPath", assetPath), ("exception", e.Message));
            }
        }
        return null;
    }

    public override AssetRequest<T> LoadAssetAsync<T>(string assetName, Action<T> assetLoadedCB)
    {
        string assetPath = GetAssetFullPath<T>(assetName);
        T asset = Loadasset<T>(assetName);
        if (asset) {
            SimulateLoadAsyncRequest<T> request = new SimulateLoadAsyncRequest<T>(assetName, assetPath, assetLoadedCB, asset);
            return request;
        }
        return null;
    }
}
