public class UnityLoadAsyncRequest<TAsset> : AssetRequest<TAsset> where TAsset : UnityEngine.Object
{
    public UnityLoadAsyncRequest(string assetFullPath) : base(assetFullPath)
    {
    }

    public override bool isLoaded => false;

    protected override bool CheckLoadDone()
    {
        return true;
    }
}
