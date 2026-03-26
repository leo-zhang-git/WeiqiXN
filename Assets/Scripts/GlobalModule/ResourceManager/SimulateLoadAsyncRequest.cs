using System;

public class SimulateLoadAsyncRequest<T> : AssetRequest<T> where T : UnityEngine.Object
{
    private T asset;
    private bool _isDone = false;
    public override bool isDone => _isDone;

    public SimulateLoadAsyncRequest(string assetName, string assetPath, Action<T> assetLoadedCB, T asset) : base(assetName, assetPath, assetLoadedCB)
    {
        this.asset = asset;
    }

    public override void Update()
    {
        // 随机数模拟异步加载
        if (!isDone && UnityEngine.Random.value > 0.5f) {
            _isDone = true;
            if (assetLoadedCB != null) {
                assetLoadedCB.Invoke(asset);
            }
        }
    }
}
