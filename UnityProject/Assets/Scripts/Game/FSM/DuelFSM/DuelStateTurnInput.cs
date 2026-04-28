public class DuelStateTurnInput : DuelFSMState
{
    public override string stateName => DuelStateDefine.STATE_TURN_INPUT;
    private SecondIntervalTimer turnTimer;

    public DuelStateTurnInput(DuelFSM fsm) : base(fsm)
    {

    }

    public override void OnEnterState()
    {
        base.OnEnterState();

        var compDuel = fsm.scene.GetComponent<SceneComponentDuel>();
        if (compDuel == null) return;

        Player curPlayer = fsm.scene.GetEntity<Player>(compDuel.curTurnPlayerGuid.value);
        if (curPlayer != null) {
            var compDuelInfo = curPlayer.GetComponent<ComponentDuelInfo>();
            compDuelInfo.turnLeftTimes.value = 30;
            turnTimer = fsm.scene.SetSecondInterval(1, OnTurnPassSecond);
        }
    }

    public override void OnUpdateState()
    {
        base.OnUpdateState();

        var compDuel = fsm.scene.GetComponent<SceneComponentDuel>();
        if (compDuel == null) return;

        Player curPlayer = fsm.scene.GetEntity<Player>(compDuel.curTurnPlayerGuid.value);
        if (curPlayer != null) {
            var compDuelInfo = curPlayer.GetComponent<ComponentDuelInfo>();
            if (compDuelInfo != null && compDuelInfo.turnLeftTimes.value <= 0) {
                fsm.SetParamterTrigger(DuelParamDefine.TRIGGER_PARAM_TURN_TIMEOUT);
            }
        }
    }

    public override void OnExitState()
    {
        base.OnExitState();

        if (turnTimer != null) {
            turnTimer.StopTimer();
        }
    }

    public void OnTurnPassSecond()
    {
        var compDuel = fsm.scene.GetComponent<SceneComponentDuel>();
        if (compDuel == null) return;

        Player curPlayer = fsm.scene.GetEntity<Player>(compDuel.curTurnPlayerGuid.value);
        if (curPlayer != null) {
            var compDuelInfo = curPlayer.GetComponent<ComponentDuelInfo>();
            if (compDuelInfo != null) {
                compDuelInfo.turnLeftTimes.value -= 1;
            }
        }
    }
}
