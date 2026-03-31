using System;
using System.Collections.Generic;

public class SceneBase : ITimerAttacher, IEventReceiver
{
    public bool isLoaded;
    private List<SystemBase> systemList = new List<SystemBase>();
    private HashSet<string> systemNames = new HashSet<string>();

    protected virtual void OnUpdate()
    {
        foreach (var system in systemList) {
            system.OnUpdate();
        }
    }

    protected virtual void OnDestroy()
    {
        OnTimerAttacherDestroyed();
    }

    public void Update()
    {
        if (!isLoaded) {
            return;
        }

        OnUpdate();
    }

    public void Destroy()
    {

    }

    #region Timer
    private List<string> _attachedTimerIds = new List<string>();
    public List<string> attachedTimerIds => _attachedTimerIds;

    public void SetSecondTimeout(float targetSeconds, Action timerCB)
    {
        Global.Instance.timerManager.SetSecondTimeout(this, targetSeconds, timerCB);
    }

    public void SetSecondInterval(float intervalSeconds, Action timerCB, int targetRepeatTimes = -1, float firstDelaySeconds = 0)
    {
        Global.Instance.timerManager.SetSecondInterval(this, intervalSeconds, timerCB, targetRepeatTimes, firstDelaySeconds);
    }

    public void SetFrameTimeout(int targetFrames, Action timerCB)
    {
        Global.Instance.timerManager.SetFrameTimeout(this, targetFrames, timerCB);
    }

    public void SetFrameInterval(int intervalFrames, Action timerCB, int targetRepeatTimes = -1, int firstDelayFrames = 0)
    {
        Global.Instance.timerManager.SetFrameInterval(this, intervalFrames, timerCB, targetRepeatTimes, firstDelayFrames);
    }

    public void OnTimerAttacherDestroyed()
    {
        Global.Instance.timerManager.RemoveTimersByAttacher(this);
    }
    #endregion

    #region Event
    private List<ISystemEventHandler> _registeredSystemEventHandlers = new List<ISystemEventHandler>();
    private List<IEntityEventHandler> _registeredEntityEventHandlers = new List<IEntityEventHandler>();
    public List<ISystemEventHandler> registeredSystemEventHandlers => _registeredSystemEventHandlers;
    public List<IEntityEventHandler> registeredEntityEventHandlers => _registeredEntityEventHandlers;

    public void EmitSystemEvent<TEvent>(TEvent systemEvent) where TEvent : SystemEventBase
    {
        Global.Instance.eventManager.EmitSystemEvent(systemEvent);
    }

    public void RegisterSystemEvent<TEvent>(Action<TEvent> eventCB) where TEvent : SystemEventBase
    {
        Global.Instance.eventManager.RegisterSystemEvent(this, eventCB);
    }

    public void UnregisterSystemEvent(ISystemEventHandler handler)
    {
        Global.Instance.eventManager.UnregisterSystemEvent(handler);
    }

    public void EmitEntityEvent<TEntity, TEvent>(TEntity entity, TEvent entityEvent) where TEntity : EntityBase where TEvent : EntityEventBase
    {
        Global.Instance.eventManager.EmitEntityEvent(entity, entityEvent);
    }

    public void RegisterEntityEvent<TEntity, TEvent>(Action<TEntity, TEvent> eventCB) where TEntity : EntityBase where TEvent : EntityEventBase
    {
        Global.Instance.eventManager.RegisterEntityEvent(this, eventCB);
    }

    public void UnregisterEntityEvent(IEntityEventHandler handler)
    {
        Global.Instance.eventManager.UnregisterEntityEvent(handler);
    }

    public void OnEventReceiverDestroyed()
    {
        Global.Instance.eventManager.UnregisterEventsByReceiver(this);
    }
    #endregion

    protected void AddSystem(SystemBase system)
    {
        if (systemNames.Contains(system.systemName)) {
            Logger.LogError("Duplicated system add to same scene. systemName:{system.systemName}", ("systemName", system.systemName));
            return;
        }
        systemList.Add(system);
        systemNames.Add(system.systemName);
    }
}
