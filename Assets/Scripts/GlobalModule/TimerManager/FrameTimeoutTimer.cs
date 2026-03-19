using System;

public class FrameTimeoutTimer : BaseTimer
{
    private readonly int targetFrames;

    private int accumulateFrames;

    public FrameTimeoutTimer(ITimerAttacher owner, string timerId, Action timerCB, int targetFrames) : base(owner, timerId, timerCB)
    {
        this.targetFrames = targetFrames;
    }

    public override void OnTimerStart()
    {
        accumulateFrames = 0;
    }

    public override void OnTimerUpdate()
    {
        if (isStopped) return;

        accumulateFrames += 1;
        if (accumulateFrames >= targetFrames) {
            OnTimerEnd();
        }
    }

    public override void OnTimerEnd()
    {
        if (timerCB != null) {
            timerCB.Invoke();
        }
        Global.Instance.timerManager.RemoveTimer(timerId);
    }
}