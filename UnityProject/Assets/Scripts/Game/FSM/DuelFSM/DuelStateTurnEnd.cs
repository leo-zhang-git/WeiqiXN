public class DuelStateTurnEnd : DuelFSMState
{
    public override string stateName => DuelStateDefine.STATE_TURN_END;

    public DuelStateTurnEnd(DuelFSM fsm) : base(fsm)
    {

    }

    public override void OnEnterState()
    {
        base.OnEnterState();

        var compDuel = fsm.scene.GetComponent<SceneComponentDuel>();
        if (compDuel != null) {
            if (compDuel.curTurnPlayerGuid.value == compDuel.player1Guid.value) {
                compDuel.curTurnPlayerGuid.value = compDuel.player2Guid.value;
            } else {
                compDuel.curTurnPlayerGuid.value = compDuel.player1Guid.value;
            }
        }

        fsm.SetParamterTrigger(DuelParamDefine.TRIGGER_PARAM_TURN_START);
    }
}
