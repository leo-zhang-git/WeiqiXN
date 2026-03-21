using System;

public interface ISystemEventHandler
{
    string eventName { get; }
    IEventReceiver receiver { get; }
    public void Execute(SystemEventBase systemEvent);
}

public class SystemEventHandler<TEvent> : ISystemEventHandler where TEvent : SystemEventBase
{
    public IEventReceiver receiver { get; private set; }
    public Action<TEvent> callback;
    public string eventName => typeof(TEvent).Name;

    public SystemEventHandler(IEventReceiver receiver, Action<TEvent> callback)
    {
        this.receiver = receiver;
        this.callback = callback;
    }

    public void Execute(SystemEventBase systemEvent)
    {
        if (systemEvent is TEvent tEvent) {
            callback?.Invoke(tEvent);
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
