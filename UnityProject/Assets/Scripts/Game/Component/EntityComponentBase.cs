public class EntityComponentBase : SavableObj
{
    [SkipSavableCheck]
    public readonly EntityBase owner;

    public EntityComponentBase(EntityBase owner)
    {
        this.owner = owner;
    }

    public virtual void OnDestroy()
    {

    }
}

