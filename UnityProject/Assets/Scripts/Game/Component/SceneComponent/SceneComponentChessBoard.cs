using Cinemachine;
using XNClient.ChessBoard;

public struct ChessInfo
{
    public string chessGuid;
    public int chessFlag;

    public void Clear()
    {
        chessGuid = string.Empty;
        chessFlag = 0;
    }
}

public class SceneComponentChessBoard : SceneComponentBase
{
    public SavableField<string> boardCfgId = SavableFieldFactory.CreateStringField(string.Empty);

    public RectGrid chessBoardGrid;
    public ChessInfo[] chessInfoMap;
    public ChessInfo[] lastChessInfoMap;
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

    public bool CheckChessFlagChanged()
    {
        for (int i = 0; i < chessInfoMap.Length; i++) {
            if (lastChessInfoMap[i].chessFlag != chessInfoMap[i].chessFlag) {
                return true;
            }
        }
        return false;
    }
}
