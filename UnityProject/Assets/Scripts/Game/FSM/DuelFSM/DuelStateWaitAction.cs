public class DuelStateWaitAction : FSMStateBase
{
    public override string stateName => DuelStateDefine.STATE_WAIT_ACTION;

    public DuelStateWaitAction(FSMBase fsm) : base(fsm)
    {

    }
}
