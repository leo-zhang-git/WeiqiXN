using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XNClient.Logger;

public class AssetBundleLoader : ResourceLoaderBase
{
    public Dictionary<string, AssetBundle> bundleDict = new Dictionary<string, AssetBundle>(System.StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> path2BundleName = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);

    public AssetBundleLoader()
    {
        if (!Directory.Exists(GlobalConfig.PATH_ASSET_BUNDLE)) {
            XNLogger.LogError("Asset bundle directory not found.", ("bundleDir", GlobalConfig.PATH_ASSET_BUNDLE));
            return;
        }

        foreach (string filePath in Directory.GetFiles(GlobalConfig.PATH_ASSET_BUNDLE)) {
            if (filePath.EndsWith(".manifest") || filePath.EndsWith(".meta")) {
                continue;
            }

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
            return bundle.LoadAssetAsync<T>(assetFullPath) as AssetRequest<T>;
        } else {
            XNLogger.LogError("Target asset not exists, load asset async failed.", ("assetFullpath", assetFullPath));
            return null;
        }
    }

    private bool TryGetAssetBundleWithPath(string assetFullPath, out AssetBundle bundle)
    {
        bundle = null;
        if (path2BundleName.TryGetValue(assetFullPath, out string bundleName)) {
            if (bundleDict.TryGetValue(bundleName, out bundle) && bundle != null) {
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
