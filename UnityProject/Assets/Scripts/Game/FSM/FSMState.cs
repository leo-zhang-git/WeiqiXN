using System.Collections.Generic;

public abstract class FSMState
{

    public string stateName;
    public FSMBase fsm;
    public List<FSMTransition> transitionList = new List<FSMTransition>();

    public FSMState(string stateName, FSMBase fsm)
    {
        this.stateName = stateName;
        this.fsm = fsm;
    }

    public virtual void OnEnterState()
    {
        fsm.curState = this;
    }

    public virtual void OnUpdateState()
    {
        TryActivateTransitions();
    }

    public virtual void OnExitState()
    {
        if (fsm.curState != null && fsm.curState.stateName == stateName) {
            fsm.curState = null;
        }
    }

    public void TryActivateTransitions()
    {
        foreach (var transition in transitionList) {
            if (transition.CheckActivateTransition()) {
                transition.ActivateTransition();
                break;
            }
        }
    }
}
