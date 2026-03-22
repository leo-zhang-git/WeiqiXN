using System;

public abstract class UILogicBase : ITimerAttacher, IEventReceiver
{
    protected virtual void OnLoaded()
    {

    }

    protected virtual void OnOpen()
    {

    }

    protected virtual void OnShow()
    {

    }

    protected virtual void OnHide()
    {

    }

    protected virtual void OnClose()
    {
        OnTimerAttacherDestroyed();
        OnEventReceiverDestroyed();
    }

    #region Timer
    public void SetSecondTimeout(float targetSeconds, Action timerCB)
    {
        throw new NotImplementedException();
    }

    public void SetSecondInterval(float intervalSeconds, Action timerCB, int targetRepeatTimes = -1, float firstDelaySeconds = 0)
    {
        throw new NotImplementedException();
    }

    public void SetFrameTimeout(int targetFrames, Action timerCB)
    {
        throw new NotImplementedException();
    }

    public void SetFrameInterval(int intervalFrames, Action timerCB, int targetRepeatTimes = -1, int firstDelayFrames = 0)
    {
        throw new NotImplementedException();
    }

    public void OnTimerAttacherDestroyed()
    {
        Global.Instance.timerManager.RemoveTimersByAttacher(this);
    }
    #endregion

    #region Event
    public void OnEventReceiverDestroyed()
    {
        Global.Instance.eventManager.UnregisterEventsByReceiver(this);
    }
    #endregion
}
