public class DuelStateTurnEnd : FSMStateBase
{
    public override string stateName => DuelStateDefine.STATE_TURN_END;

    public DuelStateTurnEnd(FSMBase fsm) : base(fsm)
    {

    }
}
