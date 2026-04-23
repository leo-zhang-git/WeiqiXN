public enum FSMBoolConditionOption
{
    False = 0,
    True = 1,
}

public class FSMTransConditionBool : FSMTransConditionBase
{
    public FSMBoolConditionOption opt;

    public FSMTransConditionBool(FSMTransition transition, string paramName, FSMBoolConditionOption opt) : base(transition, paramName)
    {
        this.opt = opt;
    }

    public override bool CheckConditionPass()
    {
        if (fsm.boolParamDict.TryGetValue(paramName, out bool paramVal)) {
            switch (opt) {
                case FSMBoolConditionOption.False:
                    return paramVal == false;
                case FSMBoolConditionOption.True:
                    return paramVal == true;
            }
        }

        return false;
    }
}
