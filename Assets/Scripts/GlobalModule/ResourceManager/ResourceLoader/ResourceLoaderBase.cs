public abstract class ResourceLoaderBase
{
    public abstract bool CheckAssetExisits(string assetFullPath);
    public abstract T Loadasset<T>(string assetName) where T : UnityEngine.Object;
    public abstract AssetRequest<T> LoadAssetAsync<T>(string assetName) where T : UnityEngine.Object;
}
