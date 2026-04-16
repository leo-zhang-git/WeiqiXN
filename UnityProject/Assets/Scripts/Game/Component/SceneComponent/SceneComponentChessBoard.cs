using Cinemachine;
using XNClient.ChessBoard;

public class SceneComponentChessBoard : SceneComponentFixed<DuelScene>
{
    public SavableField<string> boardCfgId = SavableFieldFactory.CreateStringField(string.Empty);
    public RectGrid chessBoardGrid;
    public CinemachineVirtualCamera duelVCam;

    public SceneComponentChessBoard(DuelScene owner) : base(owner)
    {

    }
}
