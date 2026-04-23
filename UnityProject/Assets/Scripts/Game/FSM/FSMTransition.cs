using System.Collections.Generic;
using XNClient.Logger;

public class FSMTransition
{
    public FSMState srcState;
    public FSMState dstState;
    public FSMBase fsm => srcState.fsm;

    public List<FSMTransConditionBase> conditionList = new List<FSMTransConditionBase>();

    public FSMTransition(FSMState srcState, FSMState dstState)
    {
        this.srcState = srcState;
        this.dstState = dstState;
    }

    public bool CheckActivateTransition()
    {
        bool allConditionPass = true;
        foreach (FSMTransConditionBase condition in conditionList) {
            if (!condition.CheckConditionPass()) {
                allConditionPass = false;
                break;
            }
        }

        return allConditionPass;
    }

    public void ActivateTransition()
    {
        srcState.OnExitState();
        dstState.OnEnterState();
        XNLogger.LogInfo("FSM activate transition.", ("srcStateName", srcState.stateName), ("dstStateName", dstState.stateName));
    }
}
