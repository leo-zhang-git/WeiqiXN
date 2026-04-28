using System;
using System.Collections.Generic;

public interface ITimerAttacher
{
    List<string> attachedTimerIds { get; }

    public SecondTimeoutTimer SetSecondTimeout(float targetSeconds, Action timerCB);

    public SecondIntervalTimer SetSecondInterval(float intervalSeconds, Action timerCB, int targetRepeatTimes = -1, float firstDelaySeconds = 0);

    public FrameTimeoutTimer SetFrameTimeout(int targetFrames, Action timerCB);

    public FrameIntervalTimer SetFrameInterval(int intervalFrames, Action timerCB, int targetRepeatTimes = -1, int firstDelayFrames = 0);

    public void OnTimerAttacherDestroyed();
}
