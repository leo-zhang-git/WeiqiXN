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
    public string eventType => typeof(TEvent).Name;

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
            Logger.LogError("Type not matched, execute system event failed.", ("dstEvent", typeof(TEvent).Name), ("srcEvent", systemEvent.GetType().Name));
        }
    }
}

public abstract class SystemEventBase
{

}

public class SystemEventTest : SystemEventBase
{

    public int param1;
}
