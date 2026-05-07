using System;
using XNClient.Logger;

public interface IEntityEventHandler
{
    string entityType { get; }
    string eventType { get; }
    IEventReceiver receiver { get; }
    bool CanHandle(EntityBase entity);
    public void Execute(EntityBase entity, EntityEventBase entityEvent);
}

public class EntityEventHandler<TEntity, TEvent> : IEntityEventHandler where TEntity : EntityBase where TEvent : EntityEventBase
{
    public IEventReceiver receiver { get; private set; }
    public Action<TEntity, TEvent> callback;
    public string entityType => EntityBase.GetEntityType<TEntity>();
    public string eventType => EntityEventBase.GetEventType<TEvent>();

    public EntityEventHandler(IEventReceiver receiver, Action<TEntity, TEvent> callback)
    {
        this.receiver = receiver;
        this.callback = callback;
    }

    public bool CanHandle(EntityBase entity)
    {
        return entity is TEntity;
    }

    public void Execute(EntityBase entity, EntityEventBase entityEvent)
    {
        if (entity is TEntity tEntity && entityEvent is TEvent tEvent) {
            callback?.Invoke(tEntity, tEvent);
        } else {
            XNLogger.LogError("Type not matched, execute entity event failed.",
                ("dstEntity", EntityBase.GetEntityType<TEntity>()), ("dstEvent", EntityEventBase.GetEventType<TEvent>()),
                ("srcEntity", entity.entityType), ("srcEvent", entityEvent.GetEventType())
            );
        }
    }
}

public abstract class EntityEventBase
{
    public static string GetEventType<TEvent>() where TEvent : EntityEventBase
    {
        return typeof(TEvent).Name;
    }

    public abstract string GetEventType();
}

public class OnEntityCreated : EntityEventBase
{
    public override string GetEventType() => EntityEventBase.GetEventType<OnEntityCreated>();
}

public class OnEntityDestroyed : EntityEventBase
{
    public override string GetEventType() => EntityEventBase.GetEventType<OnEntityDestroyed>();
}
