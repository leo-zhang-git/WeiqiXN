using System;
using System.Collections.Generic;
using UnityEngine;
using XNLogger = XNClient.Logger.XNLogger;

public class TimerManager : ModuleBase
{
    private Dictionary<string, TimerBase> timerDict = new Dictionary<string, TimerBase>();
    private HashSet<string> pendingDeleteTimerIds = new HashSet<string>();
    private enum TimerType
    {
        SecondTimeout = 0,
        SecondInterval = 1,
        FrameTimeout = 2,
        FrameInterval = 3,
    }

    public override void Init()
    {
        timerDict.Clear();
        SecondTimeoutTimer.timerIdx = 0;
        SecondIntervalTimer.timerIdx = 0;
        FrameTimeoutTimer.timerIdx = 0;
        FrameIntervalTimer.timerIdx = 0;
    }

    public override void Update()
    {
        base.Update();
        foreach (var timer in timerDict.Values) {
            if (timer.isStopped) {
                pendingDeleteTimerIds.Add(timer.timerId);
            } else {
                timer.OnTimerUpdate();
            }
        }
        foreach (var timerId in pendingDeleteTimerIds) {
            timerDict.Remove(timerId);
        }
        pendingDeleteTimerIds.Clear();
    }

    public override void OnDestroy()
    {
        timerDict.Clear();
        base.OnDestroy();
    }

    public SecondTimeoutTimer SetSecondTimeout(ITimerAttacher timerAttacher, float targetSeconds, Action timerCB)
    {
        string timerId = GenerateTimerId(TimerType.SecondTimeout);
        SecondTimeoutTimer timer = new SecondTimeoutTimer(timerAttacher, timerId, timerCB, targetSeconds);

        if (timerDict.TryAdd(timerId, timer)) {
            timerAttacher.attachedTimerIds.Add(timerId);
            timer.OnTimerStart();
            return timer;
        } else {
            XNLogger.LogError("Carete second timeout timer failed.", ("timerId", timerId), ("attacherType", timerAttacher.GetType().Name));
            return null;
        }
    }

    public SecondIntervalTimer SetSecondInterval(ITimerAttacher timerAttacher, float intervalSeconds, Action timerCB, int targetRepeatTimes, float firstDelaySeconds)
    {
        string timerId = GenerateTimerId(TimerType.SecondInterval);
        SecondIntervalTimer timer = new SecondIntervalTimer(timerAttacher, timerId, timerCB, intervalSeconds, targetRepeatTimes, firstDelaySeconds);

        if (timerDict.TryAdd(timerId, timer)) {
            timerAttacher.attachedTimerIds.Add(timerId);
            timer.OnTimerStart();
            return timer;
        } else {
            XNLogger.LogError("Carete second interval timer failed.", ("timerId", timerId), ("attacherType", timerAttacher.GetType().Name));
            return null;
        }
    }

    public FrameTimeoutTimer SetFrameTimeout(ITimerAttacher timerAttacher, int targetFrames, Action timerCB)
    {
        string timerId = GenerateTimerId(TimerType.FrameTimeout);
        FrameTimeoutTimer timer = new FrameTimeoutTimer(timerAttacher, timerId, timerCB, targetFrames);

        if (timerDict.TryAdd(timerId, timer)) {
            timerAttacher.attachedTimerIds.Add(timerId);
            timer.OnTimerStart();
            return timer;
        } else {
            XNLogger.LogError("Carete frame timeout timer failed.", ("timerId", timerId), ("attacherType", timerAttacher.GetType().Name));
            return null;
        }
    }

    public FrameIntervalTimer SetFrameInterval(ITimerAttacher timerAttacher, int intervalFrames, Action timerCB, int targetRepeatTimes, int firstDelayFrames)
    {
        string timerId = GenerateTimerId(TimerType.FrameInterval);
        FrameIntervalTimer timer = new FrameIntervalTimer(timerAttacher, timerId, timerCB, intervalFrames, targetRepeatTimes, firstDelayFrames);

        if (timerDict.TryAdd(timerId, timer)) {
            timerAttacher.attachedTimerIds.Add(timerId);
            timer.OnTimerStart();
            return timer;
        } else {
            XNLogger.LogError("Carete frame timeout timer failed.", ("timerId", timerId), ("attacherType", timerAttacher.GetType().Name));
            return null;
        }
    }

    public void RemoveTimer(string timerId)
    {
        if (timerDict.TryGetValue(timerId, out TimerBase timer)) {
            timer.StopTimer();
            timer.owner.attachedTimerIds.Remove(timerId);
            pendingDeleteTimerIds.Add(timerId);
        }
    }

    public void RemoveTimersByAttacher(ITimerAttacher timerAttacher)
    {
        foreach (string timerId in timerAttacher.attachedTimerIds) {
            if (timerDict.TryGetValue(timerId, out var timer)) {
                timer.StopTimer();
                pendingDeleteTimerIds.Add(timerId);
            }
        }
        timerAttacher.attachedTimerIds.Clear();
    }

    private string GenerateTimerId(TimerType type)
    {
        string timerId = string.Empty;
        switch (type) {
            case TimerType.SecondTimeout:
                timerId = $"SecondTimeout_{Time.realtimeSinceStartup}_{SecondTimeoutTimer.timerIdx++}";
                break;
            case TimerType.SecondInterval:
                timerId = $"SecondInterval_{Time.realtimeSinceStartup}_{SecondIntervalTimer.timerIdx++}";
                break;
            case TimerType.FrameTimeout:
                timerId = $"FrameTimeout_{Time.realtimeSinceStartup}_{FrameTimeoutTimer.timerIdx++}";
                break;
            case TimerType.FrameInterval:
                timerId = $"FrameInterval_{Time.realtimeSinceStartup}_{FrameIntervalTimer.timerIdx++}";
                break;
        }
        return timerId;
    }
}

