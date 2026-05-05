using Newtonsoft.Json.Linq;
using System.IO;
using UnityEngine;
using XNClient.Logger;

public static class GlobalConfig
{
    public static string INGAME_DEBUG_CONSOLE_PREFAB_CONFIG_ID = "IngameDebugConsole";
    public readonly static string PATH_START_SCENE = "Assets/Scenes/Main.unity";
    public readonly static string PATH_ASSET_BUNDLE = Application.streamingAssetsPath + "/AssetBundles";
    public const string CONFIG_JSON_BUNDLE_NAME = "config_json";
    private readonly static string PATH_CONFIG_JSON = Application.dataPath + "/Config/DataJson";

#if UNITY_EDITOR
    public static JObject GetJsonConfigJObject(string jsonConfigName)
    {
        string jsonPath = Path.Combine(GlobalConfig.PATH_CONFIG_JSON, jsonConfigName, jsonConfigName + ".json");
        if (File.Exists(jsonPath)) {
            return JObject.Parse(File.ReadAllText(jsonPath));
        } else {
            XNLogger.LogError("Json config asset not found.", ("jsonConfigName", jsonConfigName));
            return null;
        }
    }
#else
    public static JObject GetJsonConfigJObject(string jsonConfigName)
    {
        if (Global.Instance.resourceManager.bundleDict.TryGetValue(CONFIG_JSON_BUNDLE_NAME, out var bundle)) {
            string jsonPath = $"Assets/Config/DataJson/{jsonConfigName}/{jsonConfigName}.json";
            TextAsset jsonAsset = bundle.LoadAsset<TextAsset>(jsonPath);
            if (jsonAsset != null) {
                return JObject.Parse(jsonAsset.text);
            }
            XNLogger.LogError("Json config asset not found in asset bundle.", ("jsonConfigName", jsonConfigName));
        } else {
            XNLogger.LogError("Json config bundle not found.", ("bundleName", CONFIG_JSON_BUNDLE_NAME));
        }

        return null;
    }
#endif

    public static bool ENABLE_SIMULATE_LOAD_RESOURCE_SYNC = true;
}
