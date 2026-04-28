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

        Player curPlayer = fsm.scene.GetEntity<Player>(fsm.scene.compDuel.curTurnPlayerGuid.value);
        if (curPlayer != null) {
            curPlayer.compDuelInfo.turnLeftTimes.value = 30;
            turnTimer = fsm.scene.SetSecondInterval(1, OnTurnPassSecond);
        }
    }

    public override void OnUpdateState()
    {
        base.OnUpdateState();

        Player curPlayer = fsm.scene.GetEntity<Player>(fsm.scene.compDuel.curTurnPlayerGuid.value);
        if (curPlayer != null) {
            if (curPlayer.compDuelInfo.turnLeftTimes.value <= 0) {
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
        Player curPlayer = fsm.scene.GetEntity<Player>(fsm.scene.compDuel.curTurnPlayerGuid.value);
        if (curPlayer != null) {
            curPlayer.compDuelInfo.turnLeftTimes.value -= 1;
        }
    }
}
