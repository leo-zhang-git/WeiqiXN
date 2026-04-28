using Cinemachine;
using XNClient.ChessBoard;

public class SceneComponentChessBoard : SceneComponentBase
{
    public SavableField<string> boardCfgId = SavableFieldFactory.CreateStringField(string.Empty);

    public RectGrid chessBoardGrid;
    public CinemachineVirtualCamera duelVCam;

    public SceneComponentChessBoard(DuelScene scene) : base(scene)
    {

    }
}
