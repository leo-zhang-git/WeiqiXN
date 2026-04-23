public enum FSMFloatConditionOption
{
    NotEqual = 0,
    Equal = 1,
    Greater = 2,
    Less = 3,
}

public class FSMTransConditionFloat : FSMTransConditionBase
{
    public FSMFloatConditionOption opt;
    public float conditionVal;

    public FSMTransConditionFloat(FSMTransition transition, string paramName, FSMFloatConditionOption opt, float conditionVal) : base(transition, paramName)
    {
        this.opt = opt;
        this.conditionVal = conditionVal;
    }

    public override bool CheckConditionPass()
    {
        if (fsm.floatParamDict.TryGetValue(paramName, out float paramVal)) {
            switch (opt) {
                case FSMFloatConditionOption.NotEqual:
                    return paramVal != conditionVal;
                case FSMFloatConditionOption.Equal:
                    return paramVal == conditionVal;
                case FSMFloatConditionOption.Greater:
                    return paramVal > conditionVal;
                case FSMFloatConditionOption.Less:
                    return paramVal < conditionVal;
            }
        }

        return false;
    }
}
