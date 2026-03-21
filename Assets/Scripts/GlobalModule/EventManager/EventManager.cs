using System;
using System.Collections.Generic;

public class EventManager : BaseModule
{
    private Dictionary<string, List<ISystemEventHandler>> systemEventHandlers = new Dictionary<string, List<ISystemEventHandler>>();
    private Dictionary<EntityEventType, List<EntityEventHandler>> entityEventHandlers = new Dictionary<EntityEventType, List<EntityEventHandler>>();

    public override void Init()
    {

    }

    public override void OnDestroy()
    {
        systemEventHandlers.Clear();
        entityEventHandlers.Clear();
    }

    public void EmitSystemEvent<Event>(Event systemEvent) where Event : SystemEventBase
    {
        if (systemEventHandlers.TryGetValue(typeof(Event).Name, out var handlerSet)) {
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
        if (systemEventHandlers.TryGetValue(handler.eventName, out var handlerSet)) {
            handlerSet.Remove(handler);
        }
    }

    public void EmitEntityEvent(EntityEventType eventName, EntityBase entity, EntityEventParam eventParam = null)
    {
        if (!EntityEventDefine.eventParamTypeMap.ContainsKey(eventName)) {
            Logger.LogError($"Invalid event name, emit entity event failed. eventName: {eventName}");
            return;
        }
        Type eventType = EntityEventDefine.eventParamTypeMap[eventName];
        if (eventParam != null && eventParam != null && eventParam.GetType() != eventType) {
            Logger.LogError($"Entity event instance type not match, emit entity event failed. eventName: {eventName}");
            return;
        }

        if (entityEventHandlers.TryGetValue(eventName, out var handlerSet)) {
            foreach (var handler in handlerSet) {
                // TODO
            }
        }
    }

    public EntityEventHandler RegisterEntityEvent(EntityEventType eventName, IEventReceiver receiver, Action<EntityBase, EntityEventParam> eventCB)
    {
        EntityEventHandler handler = new EntityEventHandler(eventName, receiver, eventCB);
        List<EntityEventHandler> handlerList;
        if (!entityEventHandlers.TryGetValue(eventName, out handlerList)) {
            handlerList = new List<EntityEventHandler>();
            entityEventHandlers.Add(eventName, handlerList);
        }
        handlerList.Add(handler);
        return handler;
    }

    public void UnregisterEntityEvent(EntityEventHandler handler)
    {
        if (entityEventHandlers.TryGetValue(handler.eventName, out var handlerSet)) {
            handlerSet.Remove(handler);
        }
    }
}
