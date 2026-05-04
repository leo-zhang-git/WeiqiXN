public abstract class ResourceLoaderBase
{
    public abstract T Loadasset<T>(string assetFullPath) where T : UnityEngine.Object;
    public abstract AssetRequest<T> LoadAssetAsync<T>(string assetFullPath) where T : UnityEngine.Object;

    public ResourceManager manager;

    public ResourceLoaderBase(ResourceManager manager)
    {
        this.manager = manager;
    }
}
