using System.Collections.Generic;

public abstract class ModuleBase : IEventReceiver
{
    public abstract void Init();

    public virtual void Update() { }

    public virtual void FixedUpdate() { }

    public virtual void LateUpdate() { }

    public virtual void OnDestroy() { }

    #region Event
    private List<ISystemEventHandler> _registeredSystemEventHandlers = new List<ISystemEventHandler>();
    private List<IEntityEventHandler> _registeredEntityEventHandlers = new List<IEntityEventHandler>();

    public List<ISystemEventHandler> registeredSystemEventHandlers => _registeredSystemEventHandlers;
    public List<IEntityEventHandler> registeredEntityEventHandlers => _registeredEntityEventHandlers;

    public void OnEventReceiverDestroyed()
    {
        Global.Instance.eventManager.UnregisterEventsByReceiver(this);
    }
    #endregion

    public ModuleBase()
    {
        Init();
    }
}