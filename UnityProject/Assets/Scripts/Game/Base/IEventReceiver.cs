using System.Collections.Generic;

public interface IEventReceiver
{
    List<ISystemEventHandler> registeredSystemEventHandlers { get; }
    List<IEntityEventHandler> registeredEntityEventHandlers { get; }

    public void OnEventReceiverDestroyed();
}
