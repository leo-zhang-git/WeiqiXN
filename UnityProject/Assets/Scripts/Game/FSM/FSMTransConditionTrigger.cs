public class FSMTransConditionTrigger : FSMTransConditionBase
{
    public FSMTransConditionTrigger(FSMTransition transition, string paramName) : base(transition, paramName)
    {

    }

    public override bool CheckConditionPass()
    {
        if (fsm.triggerParamDict.TryGetValue(paramName, out bool paramVal)) {
            return paramVal == true;
        }

        return false;
    }
}
