using System;
using UnityEngine;

public class SecondTimeoutTimer : TimerBase
{
    public static uint timerIdx;

    private readonly float targetSeconds;

    private float accumulateSeconds;

    public SecondTimeoutTimer(ITimerAttacher owner, string timerId, Action timerCB, float targetSeconds)
        : base(owner, timerId, timerCB)
    {
        this.targetSeconds = targetSeconds;
    }

    public override void OnTimerStart()
    {
        accumulateSeconds = 0f;
    }

    public override void OnTimerUpdate()
    {
        if (isStopped) return;

        accumulateSeconds += Time.deltaTime;
        if (accumulateSeconds >= targetSeconds) {
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
