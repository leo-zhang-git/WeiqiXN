using System;

public interface IEntityEventHandler
{
    string entityType { get; }
    string eventType { get; }
    IEventReceiver receiver { get; }
    public void Execute(EntityBase entity, EntityEventBase entityEvent);
}

public class EntityEventHandler<TEntity, TEvent> : IEntityEventHandler where TEntity : EntityBase where TEvent : EntityEventBase
{
    public IEventReceiver receiver { get; private set; }
    public Action<TEntity, TEvent> callback;
    public string entityType => typeof(TEntity).Name;
    public string eventType => typeof(TEntity).Name;

    public EntityEventHandler(IEventReceiver receiver, Action<TEntity, TEvent> callback)
    {
        this.receiver = receiver;
        this.callback = callback;
    }

    public void Execute(EntityBase entity, EntityEventBase entityEvent)
    {
        if (entity is TEntity tEntity && entityEvent is TEvent tEvent) {
            callback?.Invoke(tEntity, tEvent);
        } else {
            Logger.LogError("Type not matched, execute entity event failed.", ("dstEntity", typeof(TEntity).Name), ("dstEvent", typeof(TEvent).Name), ("srcEntity", entity.GetType().Name), ("srcEvent", entityEvent.GetType().Name));
        }
    }
}

public abstract class EntityEventBase
{

}

public class OnEntityCreated : EntityEventBase
{

}