using UnityEngine;

public static class GlobalConfig
{
    public readonly static string PATH_START_SCENE = "Assets/Scenes/Main.unity";
    public readonly static string PATH_LOG = Application.dataPath + "/../log";

    public static bool ENABLE_LOG_WIRTER = true;
    public static bool ENABLE_SIMULATE_LOAD_RESOURCE_SYNC = true;
}