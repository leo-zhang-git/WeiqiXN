public class DuelStateTurnEnd : DuelFSMState
{
    public override string stateName => DuelStateDefine.STATE_TURN_END;

    public DuelStateTurnEnd(DuelFSM fsm) : base(fsm)
    {

    }

    public override void OnEnterState()
    {
        base.OnEnterState();

        if (fsm.scene.compDuel.curTurnPlayerGuid.value == fsm.scene.compDuel.player1Guid.value) {
            fsm.scene.compDuel.curTurnPlayerGuid.value = fsm.scene.compDuel.player2Guid.value;
        } else {
            fsm.scene.compDuel.curTurnPlayerGuid.value = fsm.scene.compDuel.player1Guid.value;
        }

        fsm.SetParamterTrigger(DuelParamDefine.TRIGGER_PARAM_TURN_START);
    }
}
