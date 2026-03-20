using System;
using System.Collections.Generic;

public class SceneBase : ITimerAttacher
{
    private HashSet<string> attachedTimerIds = new HashSet<string>();

    #region timer
    public void SetSecondTimeout(float targetSeconds, Action timerCB)
    {

    }

    public void SetSecondInterval(float intervalSeconds, Action timerCB, float targetRepeatTimes = -1, float firstDelaySeconds = 0)
    {

    }

    public void SetFrameTimeout(int targetFrames, Action timerCB)
    {

    }

    public void SetFrameInterval(float intervalFrames, Action timerCB, float targetRepeatTimes = -1, float firstDelaySeconds = 0)
    {

    }

    public void onTimerRemoved(string timerId)
    {
        attachedTimerIds.Remove(timerId);
    }
    #endregion

    #region event
    public void EmitSystemEvent()
    {

    }

    public void EmitEntityEvent()
    {

    }
    #endregion
}
