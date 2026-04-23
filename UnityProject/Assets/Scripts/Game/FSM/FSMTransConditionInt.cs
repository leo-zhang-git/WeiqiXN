public enum FSMIntConditionOption
{
    NotEqual = 0,
    Equal = 1,
    Greater = 2,
    Less = 3,
}

public class FSMTransConditionInt : FSMTransConditionBase
{
    public FSMIntConditionOption opt;
    public int conditionVal;

    public FSMTransConditionInt(FSMTransition transition, string paramName, FSMIntConditionOption opt, int conditionVal) : base(transition, paramName)
    {
        this.opt = opt;
        this.conditionVal = conditionVal;
    }

    public override bool CheckConditionPass()
    {
        if (fsm.intParamDict.TryGetValue(paramName, out int paramVal)) {
            switch (opt) {
                case FSMIntConditionOption.NotEqual:
                    return paramVal != conditionVal;
                case FSMIntConditionOption.Equal:
                    return paramVal == conditionVal;
                case FSMIntConditionOption.Greater:
                    return paramVal > conditionVal;
                case FSMIntConditionOption.Less:
                    return paramVal < conditionVal;
            }
        }

        return false;
    }
}
