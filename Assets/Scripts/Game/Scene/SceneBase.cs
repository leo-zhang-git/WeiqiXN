using System;
using System.Collections.Generic;

public class SceneBase : ITimerAttacher, IEventReceiver
{
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
        Global.Instance.timerManager.RemoveTimersByAttacher(this);
    }

    public void Destroy()
    {

    }

    #region Timer
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
    #endregion

    #region Event
    private List<ISystemEventHandler> registeredSystemEventHandlers = new List<ISystemEventHandler>();
    private List<EntityEventHandler> registeredEntityEventHandlers = new List<EntityEventHandler>();

    public void EmitSystemEvent<TEvent>(TEvent systemEvent) where TEvent : SystemEventBase
    {
        Global.Instance.eventManager.EmitSystemEvent(systemEvent);
    }

    public void RegisterSystemEvent<TEvent>(Action<TEvent> eventCB) where TEvent : SystemEventBase
    {
        ISystemEventHandler handler = Global.Instance.eventManager.RegisterSystemEvent<TEvent>(this, eventCB);
        registeredSystemEventHandlers.Add(handler);
    }

    public void UnregisterSystemEvent(ISystemEventHandler handler)
    {
        Global.Instance.eventManager.UnregisterSystemEvent(handler);
        registeredSystemEventHandlers.Remove(handler);
    }

    public void EmitEntityEvent(EntityEventType eventName, EntityBase entity, EntityEventParam eventParam = null)
    {
        Global.Instance.eventManager.EmitEntityEvent(eventName, entity, eventParam);
    }

    public void RegisterEntityEvent(EntityEventType eventName, Action<EntityBase, EntityEventParam> eventCB)
    {
        EntityEventHandler handler = Global.Instance.eventManager.RegisterEntityEvent(eventName, this, eventCB);
    }

    public void UnregisterEntityEvent(EntityEventHandler handler)
    {
        Global.Instance.eventManager.UnregisterEntityEvent(handler);
        registeredEntityEventHandlers.Remove(handler);
    }
    #endregion

    protected void AddSystem(SystemBase system)
    {
        if (systemNames.Contains(system.systemName)) {
            Logger.LogError($"Duplicated system add to same scene. systemName:{system.systemName}");
            return;
        }
        systemList.Add(system);
        systemNames.Add(system.systemName);
    }
}
