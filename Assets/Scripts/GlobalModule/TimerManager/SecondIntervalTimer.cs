using System;
using UnityEngine;

public class SecondIntervalTimer : BaseTimer
{
    private readonly int targetRepeatTimes;
    private readonly float firstDelaySeconds;
    private readonly float intervalSeconds;

    private float accumulateSeconds;
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

    public SecondIntervalTimer(ITimerAttacher owner, string timerId, Action timerCB, float intervalSeconds, int targetRepeatTimes, float firstDelaySeconds)
        : base(owner, timerId, timerCB)
    {
        this.intervalSeconds = intervalSeconds;
        this.targetRepeatTimes = targetRepeatTimes;
        this.firstDelaySeconds = firstDelaySeconds;
    }

    public override void OnTimerStart()
    {
        accumulateSeconds = 0;
        accumulateRepeatTimes = 0;
    }

    public override void OnTimerUpdate()
    {
        if (isStopped) return;

        accumulateSeconds += Time.deltaTime;
        // 首轮延迟
        if (firstDelaySeconds > 0 && accumulateRepeatTimes == 0) {
            if (accumulateSeconds >= firstDelaySeconds) {
                if (timerCB != null) {
                    timerCB.Invoke();
                }
                accumulateSeconds = 0;
                accumulateRepeatTimes += 1;
                return;
            }
        }

        if (accumulateSeconds >= intervalSeconds) {
            if (timerCB != null) {
                timerCB.Invoke();
            }
            accumulateSeconds = 0;
            accumulateRepeatTimes += 1;
        }
    }

    public override void OnTimerEnd()
    {
        Global.Instance.timerManager.RemoveTimer(timerId);
    }
}
