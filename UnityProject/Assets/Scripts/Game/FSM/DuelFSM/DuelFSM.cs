public class DuelFSM : FSMBase
{
    public DuelScene scene;

    public DuelFSM(DuelScene scene)
    {
        this.scene = scene;

        // State define
        DuelStateGameStart stateGameStart = new DuelStateGameStart(this);
        DuelStateTurnStart stateTurnStart = new DuelStateTurnStart(this);
        DuelStateTurnInput stateTurnInput = new DuelStateTurnInput(this);
        DuelStateWaitAction stateWaitAction = new DuelStateWaitAction(this);
        DuelStateTurnEnd stateTurnEnd = new DuelStateTurnEnd(this);
        DuelStateGameEnd stateGameEnd = new DuelStateGameEnd(this);

        // Transition define
        FSMTransition transTurnStart = stateGameStart.AddTransition(stateTurnStart);
        transTurnStart.AddTriggerCondition(DuelParamDefine.TRIGGER_PARAM_TURN_START);
        FSMTransition transWaitTurnInput = stateTurnStart.AddTransition(stateTurnInput);
        transWaitTurnInput.AddTriggerCondition(DuelParamDefine.TRIGGER_PARAM_WAIT_TURN_INPUT);
        FSMTransition transTurnInputFinish = stateTurnInput.AddTransition(stateTurnEnd);
        transTurnInputFinish.AddTriggerCondition(DuelParamDefine.TRIGGER_PARAM_TURN_INPUT_FINISH);

        RegisterState(stateGameStart);
        RegisterState(stateTurnStart);
        RegisterState(stateTurnInput);
        RegisterState(stateWaitAction);
        RegisterState(stateTurnEnd);
        RegisterState(stateGameEnd);

        curState = stateGameStart;
    }
}
