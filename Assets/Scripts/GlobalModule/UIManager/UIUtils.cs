using System.IO;
using UnityEngine;

public static class UIUtils
{
    public readonly static string PATH_UI_SCRIPT_FOLDER = Path.Combine(Application.dataPath, "Scripts", "Game", "UI");
    public readonly static string PATH_UI_BINDER_EXPORT = Path.Combine(PATH_UI_SCRIPT_FOLDER, "Binder");
    public readonly static string PATH_UI_LOGIC_EXPORT = Path.Combine(PATH_UI_SCRIPT_FOLDER, "Logic");

    public static string GetPagePrefabPath(string pageName)
    {
        return $"UI/Prefab/Page/{pageName}";
    }

    public static string GetWidgetPrefabPath(string widgetName)
    {
        return $"UI/Prefab/Widget/{widgetName}";
    }
}
