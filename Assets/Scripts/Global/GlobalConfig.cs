using System.IO;
using UnityEngine;

public static class GlobalConfig
{
    public readonly static string PATH_START_SCENE = "Assets/Scenes/Main.unity";
    public readonly static string PATH_LOG = Application.dataPath + "/../log";

    public static bool ENABLE_LOG_WIRTER = true;
    public static bool ENABLE_SIMULATE_LOAD_RESOURCE_SYNC = true;

    public static int BASE_CANVAS_ORDER = 1000;
    public static int CONTEXT_INCREASE_CANVAS_ORDER = 100;
    public static int POPUP_INCREASE_CANVAS_ORDER = 2;

    public readonly static string PATH_UI_SCRIPT_FOLDER = Path.Combine(Application.dataPath, "Scripts", "Game", "UI");
    public readonly static string PATH_UI_BINDER_EXPORT = Path.Combine(PATH_UI_SCRIPT_FOLDER, "Binder");
    public readonly static string PATH_UI_LOGIC_EXPORT = Path.Combine(PATH_UI_SCRIPT_FOLDER, "Logic");
    public readonly static string NAME_UI_ROOT = "uiRoot";
}