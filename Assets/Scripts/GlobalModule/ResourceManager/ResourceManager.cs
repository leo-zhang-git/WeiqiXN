using System;
using System.Collections.Generic;

public class ResourceManager : ModuleBase
{
    public ResourceLoaderBase resLoader;
    private Dictionary<string, AssetRequest<UnityEngine.Object>> requestMap = new Dictionary<string, AssetRequest<UnityEngine.Object>>();

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
        List<string> pendingDeleteRequest = new List<string>();
        foreach (var requestKV in requestMap) {
            if (requestKV.Value.isDone) {
                pendingDeleteRequest.Add(requestKV.Key);
                continue;
            }
            requestKV.Value.Update();
        }
    }

    public T LoadAsset<T>(string path) where T : UnityEngine.Object
    {
        return resLoader.Loadasset<T>(path);
    }

    public AssetRequest<T> LoadAssetAsync<T>(string path, Action<T> assetLoadedCB) where T : UnityEngine.Object
    {
        return resLoader.LoadAssetAsync<T>(path, assetLoadedCB);
    }
}
