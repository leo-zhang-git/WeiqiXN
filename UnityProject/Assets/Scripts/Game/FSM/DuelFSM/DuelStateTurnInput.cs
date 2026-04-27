public class DuelStateTurnInput : DuelFSMState
{
    public override string stateName => DuelStateDefine.STATE_TURN_INPUT;

    public DuelStateTurnInput(DuelFSM fsm) : base(fsm)
    {

    }
}
