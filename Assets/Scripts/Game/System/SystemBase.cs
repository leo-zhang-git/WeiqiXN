using System;

public abstract class SystemBase
{
    public SceneBase owner;

    public SystemBase(SceneBase owner)
    {
        this.owner = owner;
    }

    public abstract void Init();

    #region Timer
    public void SetSecondTimeout(float targetSeconds, Action timerCB)
    {
        owner.SetSecondTimeout(targetSeconds, timerCB);
    }

    public void SetSecondInterval(float intervalSeconds, Action timerCB, int targetRepeatTimes = -1, float firstDelaySeconds = 0)
    {
        owner.SetSecondInterval(intervalSeconds, timerCB, targetRepeatTimes, firstDelaySeconds);
    }

    public void SetFrameTimeout(int targetFrames, Action timerCB)
    {
        owner.SetFrameTimeout(targetFrames, timerCB);
    }

    public void SetFrameInterval(int intervalFrames, Action timerCB, int targetRepeatTimes = -1, int firstDelayFrames = 0)
    {
        owner.SetFrameInterval(intervalFrames, timerCB, targetRepeatTimes, firstDelayFrames);
    }
    #endregion
}
