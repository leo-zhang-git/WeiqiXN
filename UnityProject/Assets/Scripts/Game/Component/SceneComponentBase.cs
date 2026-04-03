public class SceneComponentBase
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
