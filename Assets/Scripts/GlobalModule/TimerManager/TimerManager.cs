using System.Collections.Generic;

public class TimerManager : BaseModule
{
    private Dictionary<string, BaseTimer> timerDict = new Dictionary<string, BaseTimer>();
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

    public override void OnDestroy()
    {
        timerDict.Clear();
        base.OnDestroy();
    }
}
