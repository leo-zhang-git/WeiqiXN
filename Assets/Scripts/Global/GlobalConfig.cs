using UnityEngine;

public static class GlobalConfig
{
    public readonly static string PATH_START_SCENE = "Assets/Scenes/Main.unity";
    public readonly static string PATH_LOG = Application.dataPath + "/../log";

    public static bool ENABLE_LOG_WIRTTER = true;
    public static bool ENABLE_SIMULATE_LOAD_RESOURCE_SYNC = true;

    public static int BASE_CANVAS_ORDER = 1000;
    public static int CONTEXT_INCREASE_CANVAS_ORDER = 100;
    public static int POPUP_INCREASE_CANVAS_ORDER = 2;
}