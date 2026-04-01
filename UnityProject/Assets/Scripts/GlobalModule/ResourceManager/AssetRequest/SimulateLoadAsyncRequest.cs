public class SimulateLoadAsyncRequest<TAsset> : AssetRequest<TAsset> where TAsset : UnityEngine.Object
{
    private TAsset simulateAsset;
    private bool _isLoaded = false;
    public override bool isLoaded => _isLoaded;

    public SimulateLoadAsyncRequest(string assetFullPath, TAsset simulateAsset) : base(assetFullPath)
    {
        this.simulateAsset = simulateAsset;
    }

    protected override bool CheckLoadDone()
    {
        if (!isLoaded && UnityEngine.Random.value > 0.5f) {
            _isLoaded = true;
            _asset = simulateAsset;
            return true;
        }
        return false;
    }
}
