using System;

public interface ISystemEventHandler
{
    string eventType { get; }
    IEventReceiver receiver { get; }
    public void Execute(SystemEventBase systemEvent);
}

public class SystemEventHandler<TEvent> : ISystemEventHandler where TEvent : SystemEventBase
{
    public IEventReceiver receiver { get; private set; }
    public Action<TEvent> callback;
    public string eventType => SystemEventBase.GetEventType<TEvent>();

    public SystemEventHandler(IEventReceiver receiver, Action<TEvent> callback)
    {
        this.receiver = receiver;
        this.callback = callback;
    }

    public void Execute(SystemEventBase systemEvent)
    {
        if (systemEvent is TEvent tEvent) {
            callback?.Invoke(tEvent);
        } else {
            Logger.LogError("Type not matched, execute system event failed.", ("dstEvent", SystemEventBase.GetEventType<TEvent>()), ("srcEvent", systemEvent.GetEventType()));
        }
    }
}

public abstract class SystemEventBase
{
    public static string GetEventType<TEvent>() where TEvent : SystemEventBase
    {
        return typeof(TEvent).Name;
    }

    public abstract string GetEventType();
}

public class OnActiveSceneChanged : SystemEventBase
{
    public override string GetEventType() => GetEventType<OnActiveSceneChanged>();
}
