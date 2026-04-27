public class DuelStateGameStart : FSMStateFixed<DuelFSM>
{
    public override string stateName => DuelStateDefine.STATE_GAME_START;

    public DuelStateGameStart(DuelFSM fsm) : base(fsm)
    {

    }
}
