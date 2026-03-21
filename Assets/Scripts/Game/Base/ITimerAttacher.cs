using System;

public interface ITimerAttacher
{
    public void SetSecondTimeout(float targetSeconds, Action timerCB);

    public void SetSecondInterval(float intervalSeconds, Action timerCB, int targetRepeatTimes = -1, float firstDelaySeconds = 0);

    public void SetFrameTimeout(int targetFrames, Action timerCB);

    public void SetFrameInterval(int intervalFrames, Action timerCB, int targetRepeatTimes = -1, int firstDelayFrames = 0);

    public void OnTimerAdded(string timerId);

    public void OnTimerRemoved(string timerId);
}
