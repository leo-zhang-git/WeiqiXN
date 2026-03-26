using System;

public abstract class AssetRequest<T> where T : UnityEngine.Object
{
    public string assetName;
    public string assetPath;
    public Action<T> assetLoadedCB;
    public abstract bool isDone { get; }
    public AssetRequest(string assetName, string assetPath, Action<T> assetLoadedCB)
    {
        this.assetName = assetName;
        this.assetPath = assetPath;
        this.assetLoadedCB = assetLoadedCB;
    }

    public abstract void Update();
}
