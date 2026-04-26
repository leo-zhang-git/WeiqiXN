public class DuelStateTurnInput : FSMStateBase
{
    public override string stateName => DuelStateDefine.STATE_TURN_INPUT;

    public DuelStateTurnInput(FSMBase fsm) : base(fsm)
    {

    }
}
