public abstract class DuelFSMState : FSMStateFixed<DuelFSM>
{
    protected DuelFSMState(DuelFSM fsm) : base(fsm)
    {

    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        fsm.scene.EmitSystemEvent(new OnDuelStateChanged(stateName));
    }
}
