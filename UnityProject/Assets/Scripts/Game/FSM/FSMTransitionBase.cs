using XNClient.Logger;

public abstract class FSMTransitionBase
{
    public FSMState srcState;
    public FSMState dstState;

    public FSMTransitionBase(FSMState srcState, FSMState dstState)
    {
        this.srcState = srcState;
        this.dstState = dstState;
    }

    public abstract bool CheckActivateTransition();

    public void ActivateTransition()
    {
        srcState.OnExitState();
        dstState.OnEnterState();
        XNLogger.LogInfo("FSM activate transition.", ("srcStateName", srcState.stateName), ("dstStateName", dstState.stateName));
    }
}
