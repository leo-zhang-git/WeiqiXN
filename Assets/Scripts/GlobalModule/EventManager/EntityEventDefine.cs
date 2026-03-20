using System;
using System.Collections.Generic;

public class EntityEventHandler
{
    public string receiver;
    public HashSet<string> expectEntityTypes;
    public Action<Entity, EntityEventParam> callback;

    public EntityEventHandler(string receiver, HashSet<string> expectEntityTypes, Action<Entity, EntityEventParam> callback)
    {
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