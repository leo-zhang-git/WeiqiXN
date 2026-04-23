public abstract class FSMTransConditionBase
{
    public FSMTransition transition;
    public string paramName;
    public FSMBase fsm => transition.srcState.fsm;

    public FSMTransConditionBase(FSMTransition transition, string paramName)
    {
        this.transition = transition;
        this.paramName = paramName;
    }

    public abstract bool CheckConditionPass();
}