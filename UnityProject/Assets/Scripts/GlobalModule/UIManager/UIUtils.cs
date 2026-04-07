using System;

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

    public static UIContextType ParseUIContextType(string contextTypeStr)
    {
        if (Enum.TryParse(contextTypeStr, out UIContextType t)) {
            return t;
        } else {
            Logger.LogError("Parse ui context type string failed.", ("contextTypeStr", contextTypeStr));
            return UIContextType.General;
        }
    }

    public static int GetUIContextBaseOrder(UIContextType contextType)
    {
        int typeValue = (int)contextType;
        return typeValue * UIConfig.CONTEXT_INCREASE_CANVAS_ORDER;
    }
}
