using System.Collections.Generic;

public class ResourceManager : ModuleBase
{
    public ResourceLoaderBase resLoader;
    private Dictionary<string, AssetRequest<UnityEngine.Object>> requestMap = new Dictionary<string, AssetRequest<UnityEngine.Object>>();

    public override void Init()
    {
#if UNITY_EDITOR
        //resLoader = new AssetDatabaseLoader();
#else
        // TODO
#endif
    }
}
