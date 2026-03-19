using System;

public class FrameIntervalTimer : BaseTimer
{
    private readonly int intervalFrames;
    private readonly int targetRepeatTimes;
    private readonly int firstDelayFrames;

    private int accumulateFrames;
    private int _accumulateRepeatTimes;
    private int accumulateRepeatTimes
    {
        get
        {
            return _accumulateRepeatTimes;
        }
        set
        {
            _accumulateRepeatTimes = value;
            if (targetRepeatTimes >= 0 && _accumulateRepeatTimes >= targetRepeatTimes) {
                OnTimerEnd();
            }
        }
    }

    public FrameIntervalTimer(ITimerAttacher owner, string timerId, Action timerCB, int intervalFrames, int targetRepeatTimes, int firstDelayFrames) : base(owner, timerId, timerCB)
    {
        this.intervalFrames = intervalFrames;
        this.targetRepeatTimes = targetRepeatTimes;
        this.firstDelayFrames = firstDelayFrames;
    }

    public override void OnTimerStart()
    {
        accumulateFrames = 0;
        accumulateRepeatTimes = 0;
    }

    public override void OnTimerUpdate()
    {
        if (isStopped) return;

        accumulateFrames += 1;
        // 首轮延迟
        if (firstDelayFrames > 0 && accumulateRepeatTimes == 0) {
            if (accumulateFrames >= firstDelayFrames) {
                if (timerCB != null) {
                    timerCB.Invoke();
                }
                accumulateFrames = 0;
                accumulateRepeatTimes += 1;
                return;
            }
        }

        if (accumulateFrames >= intervalFrames) {
            if (timerCB != null) {
                timerCB.Invoke();
            }
            accumulateFrames = 0;
            accumulateRepeatTimes += 1;
        }
    }

    public override void OnTimerEnd()
    {
        Global.Instance.timerManager.RemoveTimer(timerId);
    }

}