public class DuelStateTurnStart : FSMStateBase
{
    public override string stateName => DuelStateDefine.STATE_TURN_START;

    public DuelStateTurnStart(FSMBase fsm) : base(fsm)
    {

    }

    public override void OnEnterState()
    {
        base.OnEnterState();

        fsm.SetParamterTrigger(DuelParamDefine.TRIGGER_PARAM_TURN_START);
    }
}
