using System.IO;
using UnityEngine;

public class UIGenInfo
{
    public UIComponentBinder binder;
    public string binderClsName
    {
        get
        {
            return $"{binder.gameObject.name}UI";
        }
    }
    public string logicClsName
    {
        get
        {
            return $"{binder.gameObject.name}";
        }
    }

    public bool isPage;
    public string binderExportPath;
    public string logicExportPath;

    public UIGenInfo(UIComponentBinder binder)
    {
        this.binder = binder;
        isPage = binder.GetComponent<Canvas>() != null;
        binderExportPath = Path.GetFullPath(Path.Combine(UIGenerator.UI_BINDER_EXPORT_PATH, isPage ? "Page" : "Widget", $"{binderClsName}.cs"));
        logicExportPath = Path.GetFullPath(Path.Combine(UIGenerator.UI_LOGIC_EXPORT_PATH, isPage ? "Page" : "Widget", $"{logicClsName}.cs"));
    }
}
