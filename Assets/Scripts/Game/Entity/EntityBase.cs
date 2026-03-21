using System;
using System.Collections.Generic;

public abstract class EntityBase : ITimerAttacher
{
    protected virtual void OnDestroy()
    {
        foreach (var timerId in attachedTimerIds) {
            Global.Instance.timerManager.RemoveTimer(timerId);
        }
        attachedTimerIds.Clear();
    }

    public void Destroy()
    {
        OnDestroy();
    }

    #region Timer
    private HashSet<string> attachedTimerIds = new HashSet<string>();

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

    public void OnTimerAdded(string timerId)
    {
        attachedTimerIds.Add(timerId);
    }

    public void OnTimerRemoved(string timerId)
    {
        attachedTimerIds.Remove(timerId);
    }
    #endregion
}
