using System;
using System.Collections.Generic;

public class EntityEventHandler
{
    public EntityEventType eventName;
    public IEventReceiver receiver;
    public HashSet<string> expectEntityTypes;
    public Action<EntityBase, EntityEventParam> callback;

    public EntityEventHandler(EntityEventType eventName, IEventReceiver receiver, HashSet<string> expectEntityTypes, Action<EntityBase, EntityEventParam> callback)
    {
        this.eventName = eventName;
        this.receiver = receiver;
        this.expectEntityTypes = expectEntityTypes;
        this.callback = callback;
    }
}

public enum EntityEventType
{
    OnEntityCreated,
}

public static class EntityEventDefine
{
    public readonly static Dictionary<EntityEventType, Type> eventParamTypeMap = new Dictionary<EntityEventType, Type>()
    {
        { EntityEventType.OnEntityCreated, typeof(Params_OnEntityCreated) }
    };
}

public abstract class EntityEventParam { }

public class Params_OnEntityCreated : EntityEventParam
{

}