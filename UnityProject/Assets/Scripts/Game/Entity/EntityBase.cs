using System;
using System.Collections.Generic;
using System.Linq;
using XNClient.Logger;

public abstract class EntityBase : SavableObj, ITimerAttacher
{
    public abstract string entityType { get; }
    public readonly SceneBase scene;
    public string guid;
    public Dictionary<Type, EntityComponentBase> compDict = new Dictionary<Type, EntityComponentBase>();

    public EntityBase(SceneBase scene, string guid)
    {
        this.scene = scene;
        this.guid = guid;
    }

    public static string GetEntityType<TEntity>() where TEntity : EntityBase
    {
        return typeof(TEntity).Name;
    }

    public void AddComponent<TComponent>(TComponent comp) where TComponent : EntityComponentBase
    {
        if (compDict.ContainsKey(typeof(TComponent))) {
            XNLogger.LogError("Try add duplicated component to entity, add entity component failed.", ("component", typeof(TComponent).Name));
        } else {
            compDict[typeof(TComponent)] = comp;
        }
    }

    public TComponent GetComponent<TComponent>() where TComponent : EntityComponentBase
    {
        if (compDict.TryGetValue(typeof(TComponent), out EntityComponentBase comp)) {
            return (TComponent)comp;
        } else {
            return null;
        }
    }

    #region LifeCycle
    protected virtual void OnDestroy()
    {
        OnTimerAttacherDestroyed();

        foreach (var comp in compDict.Values.ToList()) {
            comp.OnDestroy();
        }
        compDict.Clear();
    }

    public void Destroy()
    {
        scene.RemoveEntity(this);
        OnDestroy();
    }
    #endregion

    #region Timer
    private List<string> _attachedTimerIds = new List<string>();
    public List<string> attachedTimerIds => _attachedTimerIds;

    public SecondTimeoutTimer SetSecondTimeout(float targetSeconds, Action timerCB)
    {
        return Global.Instance.timerManager.SetSecondTimeout(this, targetSeconds, timerCB);
    }

    public SecondIntervalTimer SetSecondInterval(float intervalSeconds, Action timerCB, int targetRepeatTimes = -1, float firstDelaySeconds = 0)
    {
        return Global.Instance.timerManager.SetSecondInterval(this, intervalSeconds, timerCB, targetRepeatTimes, firstDelaySeconds);
    }

    public FrameTimeoutTimer SetFrameTimeout(int targetFrames, Action timerCB)
    {
        return Global.Instance.timerManager.SetFrameTimeout(this, targetFrames, timerCB);
    }

    public FrameIntervalTimer SetFrameInterval(int intervalFrames, Action timerCB, int targetRepeatTimes = -1, int firstDelayFrames = 0)
    {
        return Global.Instance.timerManager.SetFrameInterval(this, intervalFrames, timerCB, targetRepeatTimes, firstDelayFrames);
    }

    public void OnTimerAttacherDestroyed()
    {
        Global.Instance.timerManager.RemoveTimersByAttacher(this);
    }
    #endregion
}
