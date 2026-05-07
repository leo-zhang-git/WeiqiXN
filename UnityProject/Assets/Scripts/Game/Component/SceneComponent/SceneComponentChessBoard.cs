using Cinemachine;
using XNClient.ChessBoard;

public struct ChessInfo
{
    public string chessGuid;
    public int chessFlag;
}

public class SceneComponentChessBoard : SceneComponentBase
{
    public SavableField<string> boardCfgId = SavableFieldFactory.CreateStringField(string.Empty);

    public RectGrid chessBoardGrid;
    public ChessInfo[] chessInfoMap;
    public CinemachineVirtualCamera duelVCam;

    public SceneComponentChessBoard(DuelScene scene) : base(scene)
    {

    }

    public int GetPosIndexByCoords(RectCoordinates coords)
    {
        if (coords == null || chessBoardGrid == null) {
            return -1;
        }

        int gridSize = chessBoardGrid.gridSize;
        if (gridSize <= 0) {
            return -1;
        }

        if (coords.x < 0 || coords.x >= gridSize || coords.z < 0 || coords.z >= gridSize) {
            return -1;
        }

        int posIndex = coords.z * gridSize + coords.x;
        if (posIndex < 0 || posIndex >= chessInfoMap.Length) {
            return -1;
        }

        return posIndex;
    }
}
