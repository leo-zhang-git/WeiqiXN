public class UnityLoadAsyncRequest<TAsset> : AssetRequest<TAsset> where TAsset : UnityEngine.Object
{
    private UnityEngine.AssetBundleRequest unityRequest;
    private bool _isLoaded = false;
    public override bool isLoaded => _isLoaded;

    public UnityLoadAsyncRequest(string assetFullPath, UnityEngine.AssetBundleRequest unityRequest) : base(assetFullPath)
    {
        this.unityRequest = unityRequest;
    }

    protected override bool CheckLoadDone()
    {
        if (!isLoaded && unityRequest != null && unityRequest.isDone) {
            _asset = unityRequest.asset as TAsset;
            _isLoaded = true;
            return true;
        }
        return false;
    }

    public override void Dispose()
    {
        unityRequest = null;
        base.Dispose();
    }
}
