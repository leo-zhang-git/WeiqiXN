using System;
using System.Collections.Generic;

public abstract class EntityBase : ITimerAttacher
{
    public readonly SceneBase scene;
    public abstract string entityType { get; }
    public List<EntityComponentBase> compList = new List<EntityComponentBase>();

    public EntityBase(SceneBase scene)
    {
        this.scene = scene;
    }

    #region LifeCycle
    protected virtual void OnDestroy()
    {
        OnTimerAttacherDestroyed();
    }

    public void Destroy()
    {
        foreach (var comp in compList) {
            comp.OnDestroy();
        }
        OnDestroy();
        scene.RemoveEntity(this);
    }
    #endregion

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
}
