using System;

public abstract class TimerBase
{
    public readonly string timerId;
    public ITimerAttacher owner;
    protected Action timerCB;
    protected bool _isStopped;
    public bool isStopped => _isStopped;

    public TimerBase(ITimerAttacher owner, string timerId, Action timerCB)
    {
        this.owner = owner;
        this.timerId = timerId;
        this.timerCB = timerCB;
    }

    public virtual void StopTimer()
    {
        _isStopped = true;
    }

    public abstract void OnTimerStart();

    public abstract void OnTimerUpdate();

    public abstract void OnTimerEnd();
}
