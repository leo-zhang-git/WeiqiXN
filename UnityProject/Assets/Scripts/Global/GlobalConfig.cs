using UnityEngine;

public static class GlobalConfig
{
    public static string INGAME_DEBUG_CONSOLE_PREFAB_CONFIG_ID = "IngameDebugConsole";
    public readonly static string PATH_START_SCENE = "Assets/Scenes/Main.unity";
    // TODO 处理打包环境取配表的方式
    public readonly static string PATH_CONFIG_JSON = Application.dataPath + "/Config/DataJson";

    public static bool ENABLE_SIMULATE_LOAD_RESOURCE_SYNC = true;
}