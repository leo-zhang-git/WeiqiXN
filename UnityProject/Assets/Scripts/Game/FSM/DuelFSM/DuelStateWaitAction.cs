public class DuelStateWaitAction : DuelFSMState
{
    public override string stateName => DuelStateDefine.STATE_WAIT_ACTION;

    public DuelStateWaitAction(DuelFSM fsm) : base(fsm)
    {

    }
}
