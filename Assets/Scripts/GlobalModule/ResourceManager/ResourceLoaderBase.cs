using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceLoaderBase
{
    public static Dictionary<string, string> AssetExtendDict = new Dictionary<string, string>()
    {
        { typeof(GameObject).Name, "prefab" },
        { typeof(Sprite).Name, "png" },
        { typeof(Material).Name, "mat" },
    };

    public string GetAssetFullPath<T>(string path)
    {
        if (AssetExtendDict.TryGetValue(typeof(T).Name, out string ext)) {
            return path + ext;
        }
        return string.Empty;
    }

    public abstract T Loadasset<T>(string assetName) where T : UnityEngine.Object;
    public abstract AssetRequest<T> LoadAssetAsync<T>(string assetName, Action<T> assetLoadedCB) where T : UnityEngine.Object;
}
