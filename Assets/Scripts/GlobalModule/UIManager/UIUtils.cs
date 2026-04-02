public static class UIUtils
{
    public static string GetPagePrefabPath(string pageName)
    {
        return $"UI/Prefab/Page/{pageName}";
    }

    public static string GetWidgetPrefabPath(string widgetName)
    {
        return $"UI/Prefab/Widget/{widgetName}";
    }
}
