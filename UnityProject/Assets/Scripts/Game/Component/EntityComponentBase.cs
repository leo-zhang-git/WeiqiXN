public class EntityComponentBase
{
    public readonly EntityBase owner;

    public EntityComponentBase(EntityBase owner)
    {
        this.owner = owner;
        owner.compList.Add(this);
    }

    public virtual void OnDestroy()
    {

    }
}
