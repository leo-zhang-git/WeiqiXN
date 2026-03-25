using UnityEngine;

public class EditorColorScope : System.IDisposable
{
    private Color originColor;

    public EditorColorScope(Color newColor)
    {
        originColor = GUI.color;
        GUI.color = newColor;
    }

    public void Dispose()
    {
        GUI.color = originColor;
    }
}

public static class EditorUtils
{

}
