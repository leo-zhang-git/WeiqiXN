using System.IO;

public class UIBinderInfo
{
    public UIComponentBinder binder;
    public string binderClsName
    {
        get
        {
            return $"{binder.gameObject.name}UI";
        }
    }

    public bool isPage;
    public string exportPath;

    public UIBinderInfo(UIComponentBinder binder)
    {
        this.binder = binder;
        exportPath = Path.GetFullPath(Path.Combine(UIBinderGenerator.UI_BINDER_EXPORT_PATH, isPage ? "Page" : "Widget", $"{binderClsName}.cs"));
    }
}
