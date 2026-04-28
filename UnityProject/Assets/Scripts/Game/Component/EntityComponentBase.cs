public class EntityComponentBase : SavableObj
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

public class EntityComponentFixed<TEntity> : EntityComponentBase where TEntity : EntityBase
{
    [SkipSavableCheck]
    public TEntity owner;

    public EntityComponentFixed(TEntity owner) : base(owner)
    {
        this.owner = owner;
    }
}

