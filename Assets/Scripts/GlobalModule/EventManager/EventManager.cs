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

    public SystemEventHandler RegisterSystemEvent(SystemEventType eventName, IEventReceiver receiver, Action<SystemEventParam> eventCB)
    {
        SystemEventHandler handler = new SystemEventHandler(eventName, receiver, eventCB);
        HashSet<SystemEventHandler> handlerSet;
        if (!systemEventHandlers.TryGetValue(eventName, out handlerSet)) {
            handlerSet = new HashSet<SystemEventHandler>();
            systemEventHandlers.Add(eventName, handlerSet);
        }
        handlerSet.Add(handler);
        return handler;
    }

    public void UnregisterSystemEvent(SystemEventHandler handler)
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
                if (handler.expectEntityTypes.Contains(entity.entityType)) {
                    handler.callback.Invoke(entity, eventParam);
                }
            }
        }
    }

    public EntityEventHandler RegisterEntityEvent(EntityEventType eventName, IEventReceiver receiver, string expectEntityType, Action<EntityBase, EntityEventParam> eventCB)
    {
        return RegisterEntityEvent(eventName, receiver, new HashSet<string>() { expectEntityType }, eventCB);
    }

    public EntityEventHandler RegisterEntityEvent(EntityEventType eventName, IEventReceiver receiver, HashSet<string> expectEntityTypes, Action<EntityBase, EntityEventParam> eventCB)
    {
        EntityEventHandler handler = new EntityEventHandler(eventName, receiver, expectEntityTypes, eventCB);
        HashSet<EntityEventHandler> handlerSet;
        if (!entityEventHandlers.TryGetValue(eventName, out handlerSet)) {
            handlerSet = new HashSet<EntityEventHandler>();
            entityEventHandlers.Add(eventName, handlerSet);
        }
        handlerSet.Add(handler);
        return handler;
    }

    public void UnregisterEntityEvent(EntityEventHandler handler)
    {
        if (entityEventHandlers.TryGetValue(handler.eventName, out var handlerSet)) {
            handlerSet.Remove(handler);
        }
    }
}
