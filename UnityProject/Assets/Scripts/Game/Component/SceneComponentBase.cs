public abstract class SceneComponentBase : SavableObj
{
    private readonly SceneBase owner;

    public SceneComponentBase(SceneBase owner)
    {
        this.owner = owner;
        owner.compList.Add(this);
    }

    public virtual void OnDestroy()
    {

    }
}

public abstract class SceneComponentFixed<TScene> : SceneComponentBase where TScene : SceneBase
{
    public TScene owner { get; }

    protected SceneComponentFixed(SceneBase owner) : base(owner)
    {
    }
}