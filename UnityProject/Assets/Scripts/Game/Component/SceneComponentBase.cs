public abstract class SceneComponentBase : SavableObj
{
    private readonly SceneBase scene;

    public SceneComponentBase(SceneBase scene)
    {
        this.scene = scene;
        scene.compList.Add(this);
    }

    public virtual void OnDestroy()
    {

    }
}

public abstract class SceneComponentFixed<TScene> : SceneComponentBase where TScene : SceneBase
{
    [SkipSavableCheck]
    public TScene scene;

    protected SceneComponentFixed(TScene scene) : base(scene)
    {
        this.scene = scene;
    }
}