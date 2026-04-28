public abstract class SceneComponentBase : SavableObj
{
    [SkipSavableCheck]
    public readonly SceneBase scene;

    public SceneComponentBase(SceneBase scene)
    {
        this.scene = scene;
    }

    public virtual void OnDestroy()
    {

    }
}