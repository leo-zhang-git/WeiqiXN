using System;
using System.Collections.Generic;

public class EventManager : BaseModule
{
    private Dictionary<string, List<ISystemEventHandler>> systemEventHandlers = new Dictionary<string, List<ISystemEventHandler>>();
    private Dictionary<string, List<IEntityEventHandler>> entityEventHandlers = new Dictionary<string, List<IEntityEventHandler>>();

    public override void Init()
    {

    }

    public override void OnDestroy()
    {
        systemEventHandlers.Clear();
        entityEventHandlers.Clear();
    }

    public void EmitSystemEvent<TEvent>(TEvent systemEvent) where TEvent : SystemEventBase
    {
        if (systemEventHandlers.TryGetValue(typeof(TEvent).Name, out var handlerSet)) {
            foreach (var handler in handlerSet) {
                handler.Execute(systemEvent);
            }
        }
    }

    public ISystemEventHandler RegisterSystemEvent<TEvent>(IEventReceiver receiver, Action<TEvent> eventCB) where TEvent : SystemEventBase
    {
        SystemEventHandler<TEvent> handler = new SystemEventHandler<TEvent>(receiver, eventCB);
        List<ISystemEventHandler> handlerList;
        if (!systemEventHandlers.TryGetValue(typeof(TEvent).Name, out handlerList)) {
            handlerList = new List<ISystemEventHandler>();
            systemEventHandlers.Add(typeof(TEvent).Name, handlerList);
        }
        handlerList.Add(handler);
        return handler;
    }

    public void UnregisterSystemEvent(ISystemEventHandler handler)
    {
        if (systemEventHandlers.TryGetValue(handler.eventType, out var handlerSet)) {
            handlerSet.Remove(handler);
        }
    }

    public void EmitEntityEvent<TEntity, TEvent>(TEntity entity, TEvent entityEvent) where TEntity : EntityBase where TEvent : EntityEventBase
    {
        if (entityEventHandlers.TryGetValue(typeof(TEvent).Name, out var handlerSet)) {
            foreach (var handler in handlerSet) {
                if (handler.entityType == typeof(TEntity).Name) {
                    handler.Execute(entity, entityEvent);
                }
            }
        }
    }

    public IEntityEventHandler RegisterEntityEvent<TEntity, TEvent>(IEventReceiver receiver, Action<TEntity, TEvent> eventCB) where TEntity : EntityBase where TEvent : EntityEventBase
    {
        EntityEventHandler<TEntity, TEvent> handler = new EntityEventHandler<TEntity, TEvent>(receiver, eventCB);
        List<IEntityEventHandler> handlerList;
        if (!entityEventHandlers.TryGetValue(typeof(TEvent).Name, out handlerList)) {
            handlerList = new List<IEntityEventHandler>();
            entityEventHandlers.Add(typeof(TEvent).Name, handlerList);
        }
        handlerList.Add(handler);
        return handler;
    }

    public void UnregisterEntityEvent(IEntityEventHandler handler)
    {
        if (entityEventHandlers.TryGetValue(handler.eventType, out var handlerSet)) {
            handlerSet.Remove(handler);
        }
    }
}
