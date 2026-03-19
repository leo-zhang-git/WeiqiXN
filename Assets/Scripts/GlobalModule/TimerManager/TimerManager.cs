using System.Collections.Generic;

public class TimerManager : BaseModule
{
    private Dictionary<string, BaseTimer> timerDict = new Dictionary<string, BaseTimer>();
    private HashSet<string> pendingDeleteTimerIds = new HashSet<string>();
    private int timerIdx;
    private enum TimerType
    {
        Second = 0,
        Frame = 1,
    }

    public override void Init()
    {
        timerDict.Clear();
        timerIdx = 0;
    }

    public override void Update()
    {
        base.Update();
        foreach (var timer in timerDict.Values) {
            timer.OnTimerUpdate();
        }
        foreach (var timerId in pendingDeleteTimerIds) {
            timerDict.Remove(timerId);
        }
    }

    public override void OnDestroy()
    {
        timerDict.Clear();
        base.OnDestroy();
    }

    public void RemoveTimer(string timerId)
    {
        if (timerDict.TryGetValue(timerId, out BaseTimer timer)) {
            timer.isStopped = true;
            pendingDeleteTimerIds.Add(timerId);
        }
    }
}
