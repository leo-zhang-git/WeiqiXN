using System;

public abstract class BaseTimer
{
    public readonly string timerId;
    public ITimerAttacher owner;
    protected Action timerCB;

    public bool isStopped;

    public BaseTimer(ITimerAttacher owner, string timerId, Action timerCB)
    {
        this.owner = owner;
        this.timerId = timerId;
        this.timerCB = timerCB;

    }

    public abstract void OnTimerStart();

    public abstract void OnTimerUpdate();

    public abstract void OnTimerEnd();
}
