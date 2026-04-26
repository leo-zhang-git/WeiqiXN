public class DuelStateGameStart : FSMStateBase
{
    public override string stateName => DuelStateDefine.STATE_GAME_START;

    public DuelStateGameStart(FSMBase fsm) : base(fsm)
    {

    }
}
