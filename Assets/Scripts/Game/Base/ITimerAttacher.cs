using System;

public interface ITimerAttacher
{
    public void SetSecondTimeout(float targetSeconds, Action timerCB);

    public void SetSecondInterval(float intervalSeconds, Action timerCB, float targetRepeatTimes = -1, float firstDelaySeconds = 0);

    public void SetFrameTimeout(int targetFrames, Action timerCB);

    public void SetFrameInterval(float intervalFrames, Action timerCB, float targetRepeatTimes = -1, float firstDelaySeconds = 0);

    public void onTimerRemoved(string timerId);
}
