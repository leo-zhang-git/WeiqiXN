using UnityEngine;
using XNClient.Logger;

public class AssetBundleLoader : ResourceLoaderBase
{
    public AssetBundleLoader(ResourceManager manager) : base(manager)
    {

    }

    public override T Loadasset<T>(string assetFullPath)
    {
        if (TryGetAssetBundleWithPath(assetFullPath, out var bundle)) {
            return bundle.LoadAsset<T>(assetFullPath);
        } else {
            XNLogger.LogError("Target asset not exists, load asset failed.", ("assetFullpath", assetFullPath));
            return null;
        }
    }

    public override AssetRequest<T> LoadAssetAsync<T>(string assetFullPath)
    {
        if (TryGetAssetBundleWithPath(assetFullPath, out var bundle)) {
            AssetBundleRequest unityRequest = bundle.LoadAssetAsync<T>(assetFullPath);
            if (unityRequest != null) {
                return new UnityLoadAsyncRequest<T>(assetFullPath, unityRequest);
            }
            XNLogger.LogError("Unity asset bundle async request is null.", ("assetFullpath", assetFullPath));
            return null;
        } else {
            XNLogger.LogError("Target asset not exists, load asset async failed.", ("assetFullpath", assetFullPath));
            return null;
        }
    }

    private bool TryGetAssetBundleWithPath(string assetFullPath, out AssetBundle bundle)
    {
        bundle = null;
        if (manager.path2BundleName.TryGetValue(assetFullPath, out string bundleName)) {
            if (manager.bundleDict.TryGetValue(bundleName, out bundle) && bundle != null) {
                return true;
            } else {
                XNLogger.LogError("Target asset bundle not found.", ("bundleName", bundleName));
            }
        } else {
            XNLogger.LogError("Bundle name for target asset not found.", ("assetFullPath", assetFullPath));
        }
        return false;
    }
}
