using System;
using System.Collections.Generic;

public class EventManager : BaseModule
{
    private Dictionary<SystemEventType, HashSet<SystemEventHandler>> systemEventHandlers = new Dictionary<SystemEventType, HashSet<SystemEventHandler>>();
    private Dictionary<EntityEventType, HashSet<EntityEventHandler>> entityEventHandlers = new Dictionary<EntityEventType, HashSet<EntityEventHandler>>();

    public override void Init()
    {

    }

    public override void OnDestroy()
    {
        systemEventHandlers.Clear();
        entityEventHandlers.Clear();
    }

    public void EmitSystemEvent(SystemEventType eventName, SystemEventParam eventParam = null)
    {
        if (!SystemEventDefine.eventParamTypeMap.ContainsKey(eventName)) {
            Logger.LogError($"Invalid event name, emit system event failed. eventName: {eventName}");
            return;
        }
        Type eventType = SystemEventDefine.eventParamTypeMap[eventName];
        if (eventType != null && eventParam.GetType() != eventType) {
            Logger.LogError($"System event instance type not match, emit system event failed. eventName: {eventName}");
            return;
        }

        if (systemEventHandlers.TryGetValue(eventName, out var handlerSet)) {
            foreach (var handler in handlerSet) {
                handler.callback.Invoke(eventParam);
            }
        }
    }

    public SystemEventHandler RegisterSystemEvent(SystemEventType eventName, string receiver, Action<SystemEventParam> cb)
    {
        SystemEventHandler handler = new SystemEventHandler(receiver, cb);
        HashSet<SystemEventHandler> handlerSet;
        if (!systemEventHandlers.TryGetValue(eventName, out handlerSet)) {
            handlerSet = new HashSet<SystemEventHandler>();
            systemEventHandlers.Add(eventName, handlerSet);
        }
        handlerSet.Add(handler);
        return handler;
    }

    public void UnregisterSystemEvent(SystemEventType eventName, SystemEventHandler handler)
    {
        if (systemEventHandlers.TryGetValue(eventName, out var handlerSet)) {
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
        if (eventParam != null && eventParam.GetType() != eventType) {
            Logger.LogError($"Entity event instance type not match, emit entity event failed. eventName: {eventName}");
            return;
        }

        if (entityEventHandlers.TryGetValue(eventName, out var handlerSet)) {
            foreach (var handler in handlerSet) {
                // TODO
            }
        }
    }

    public EntityEventHandler RegisterEntityEvent(EntityEventType eventName, string receiver, string expectEntityType, Action<Entity, EntityEventParam> cb)
    {
        return RegisterEntityEvent(eventName, receiver, new HashSet<string>() { expectEntityType }, cb);
    }

    public EntityEventHandler RegisterEntityEvent(EntityEventType eventName, string receiver, HashSet<string> expectEntityTypes, Action<Entity, EntityEventParam> cb)
    {
        EntityEventHandler handler = new EntityEventHandler(receiver, expectEntityTypes, cb);
        HashSet<EntityEventHandler> handlerSet;
        if (!entityEventHandlers.TryGetValue(eventName, out handlerSet)) {
            handlerSet = new HashSet<EntityEventHandler>();
            entityEventHandlers.Add(eventName, handlerSet);
        }
        handlerSet.Add(handler);
        return handler;
    }
}
