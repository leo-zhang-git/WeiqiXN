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

    public static int GetUIContextBaseOrder(UIContextType contextType)
    {
        int typeValue = (int)contextType;
        return typeValue * UIConfig.CONTEXT_INCREASE_CANVAS_ORDER;
    }
}
