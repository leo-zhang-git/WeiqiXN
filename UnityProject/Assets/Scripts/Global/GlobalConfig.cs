using Newtonsoft.Json.Linq;
using System.IO;
using UnityEngine;

public static class GlobalConfig
{
    public static string INGAME_DEBUG_CONSOLE_PREFAB_CONFIG_ID = "IngameDebugConsole";
    public readonly static string PATH_START_SCENE = "Assets/Scenes/Main.unity";
#if UNITY_EDITOR
    private readonly static string PATH_CONFIG_JSON = Application.dataPath + "/Config/DataJson";
    public static JObject GetJsonConfigJObject(string jsonConfigName)
    {
        string jsonPath = Path.Combine(GlobalConfig.PATH_CONFIG_JSON, jsonConfigName, jsonConfigName + ".json");
        if (File.Exists(jsonPath)) {
            return JObject.Parse(File.ReadAllText(jsonPath));
        } else {
            return null;
        }
    }
#else
    public static JObject GetJsonConfigJObject(string jsonConfigName)
    {
        return null;
    }
#endif

    public static bool ENABLE_SIMULATE_LOAD_RESOURCE_SYNC = true;
}