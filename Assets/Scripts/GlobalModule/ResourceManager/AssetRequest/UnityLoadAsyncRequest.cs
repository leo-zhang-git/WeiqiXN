public class UnityLoadAsyncRequest<TAsset> : AssetRequest<TAsset> where TAsset : UnityEngine.Object
{
    public UnityLoadAsyncRequest(string assetFullPath) : base(assetFullPath)
    {
    }

    public override bool isLoaded => false;

    public override void Update()
    {

    }
}
