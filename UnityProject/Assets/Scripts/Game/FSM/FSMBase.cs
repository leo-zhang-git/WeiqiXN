using System.Collections.Generic;
using XNClient.Logger;

public abstract class FSMBase
{
    public FSMState curState;
    public Dictionary<string, FSMState> stateDict = new Dictionary<string, FSMState>();

    public Dictionary<string, int> intParamDict = new Dictionary<string, int>();
    public Dictionary<string, float> floatParamDict = new Dictionary<string, float>();
    public Dictionary<string, string> stringParamDict = new Dictionary<string, string>();
    public Dictionary<string, bool> boolParamDict = new Dictionary<string, bool>();
    public Dictionary<string, bool> triggerParamDict = new Dictionary<string, bool>();

    public void RegisterState(FSMState state)
    {
        if (stateDict.ContainsKey(state.stateName)) {
            XNLogger.LogError("Duplicated state name, register state for fsm failed.", ("stateName", state.stateName));
            return;
        }
        stateDict.Add(state.stateName, state);
    }

    public bool TryGetState(string stateName, out FSMState state)
    {
        if (stateDict.TryGetValue(stateName, out state)) {
            return true;
        }
        return false;
    }

    public void SetParameterInt(string paramName, int paramVal)
    {
        if (intParamDict.ContainsKey(paramName)) {
            intParamDict[paramName] = paramVal;
            if (curState != null) {
                curState.TryActivateTransitions();
            }
            XNLogger.LogInfo("FSM set int parameter.", ("paramName", paramName), ("paramVal", paramVal.ToString()));
        }
    }

    public void SetParameterFloat(string paramName, float paramVal)
    {
        if (floatParamDict.ContainsKey(paramName)) {
            floatParamDict[paramName] = paramVal;
            if (curState != null) {
                curState.TryActivateTransitions();
            }
            XNLogger.LogInfo("FSM set float parameter.", ("paramName", paramName), ("paramVal", paramVal.ToString()));
        }
    }

    public void SetParameterString(string paramName, string paramVal)
    {
        if (stringParamDict.ContainsKey(paramName)) {
            stringParamDict[paramName] = paramVal;
            if (curState != null) {
                curState.TryActivateTransitions();
            }
            XNLogger.LogInfo("FSM set string parameter.", ("paramName", paramName), ("paramVal", paramVal));
        }
    }

    public void SetParameterBool(string paramName, bool paramVal)
    {
        if (boolParamDict.ContainsKey(paramName)) {
            boolParamDict[paramName] = paramVal;
            if (curState != null) {
                curState.TryActivateTransitions();
            }
            XNLogger.LogInfo("FSM set bool parameter.", ("paramName", paramName), ("paramVal", paramVal.ToString()));
        }
    }
}
