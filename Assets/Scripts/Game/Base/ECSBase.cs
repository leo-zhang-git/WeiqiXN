using System;
using System.Collections.Generic;

public abstract class ECSBase : ITimerAttacher
{
    protected HashSet<string> attachedTimerIds = new HashSet<string>();

    public void SetFrameTimeout(int targetFrames, Action timerCB)
    {

    }

    public void SetFrameInterval(float intervalFrames, Action timerCB, float targetRepeatTimes = -1, float firstDelaySeconds = 0)
    {

    }

    public void SetSecondTimeout(float targetSeconds, Action timerCB)
    {

    }

    public void SetSecondInterval(float intervalSeconds, Action timerCB, float targetRepeatTimes = -1, float firstDelaySeconds = 0)
    {

    }

    public void onTimerRemoved(string timerId)
    {
        attachedTimerIds.Remove(timerId);
    }
}
