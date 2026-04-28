public class DuelStateTurnStart : FSMStateFixed<DuelFSM>
{
    public override string stateName => DuelStateDefine.STATE_TURN_START;

    public DuelStateTurnStart(DuelFSM fsm) : base(fsm)
    {

    }

    public override void OnEnterState()
    {
        base.OnEnterState();

        fsm.SetParamterTrigger(DuelParamDefine.TRIGGER_PARAM_WAIT_TURN_INPUT);
    }
}
