using System;
using System.Collections.Generic;

public class SystemEventHandler
{
    public string receiver;
    public Action<SystemEventParam> callback;

    public SystemEventHandler(string receiver, Action<SystemEventParam> callback)
    {
        this.receiver = receiver;
        this.callback = callback;
    }
}

public enum SystemEventType
{
    SystemEventTest = 0,
}

public static class SystemEventDefine
{
    public readonly static Dictionary<SystemEventType, Type> eventParamTypeMap = new Dictionary<SystemEventType, Type>()
    {
        { SystemEventType.SystemEventTest, typeof (SystemEventParam_Test) }
    };
}

public abstract class SystemEventParam { }

public class SystemEventParam_Test : SystemEventParam { }