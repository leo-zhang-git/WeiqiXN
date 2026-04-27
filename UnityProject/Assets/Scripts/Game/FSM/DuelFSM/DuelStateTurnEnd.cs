public class DuelStateTurnEnd : DuelFSMState
{
    public override string stateName => DuelStateDefine.STATE_TURN_END;

    public DuelStateTurnEnd(DuelFSM fsm) : base(fsm)
    {

    }
}
