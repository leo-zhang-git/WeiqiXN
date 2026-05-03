using System.Collections.Generic;

public abstract class FSMStateBase
{

    public abstract string stateName { get; }
    public FSMBase fsm;
    protected List<FSMTransition> transitionList = new List<FSMTransition>();

    public FSMStateBase(FSMBase fsm)
    {
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

    public FSMTransition AddTransition(FSMStateBase dstState)
    {
        FSMTransition transtion = new FSMTransition(this, dstState);
        transitionList.Add(transtion);
        return transtion;
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

public abstract class FSMStateFixed<TFSM> : FSMStateBase where TFSM : FSMBase
{
    public new TFSM fsm;

    public FSMStateFixed(TFSM fsm) : base(fsm)
    {
        this.fsm = fsm;
    }
}
