using Cinemachine;
using System.Collections.Generic;
using XNClient.ChessBoard;

public class SceneComponentChessBoard : SceneComponentBase
{
    public SavableField<string> boardCfgId = SavableFieldFactory.CreateStringField(string.Empty);

    public RectGrid chessBoardGrid;
    public List<int> chessFlagMap;
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
        if (posIndex < 0 || posIndex >= chessFlagMap.Count) {
            return -1;
        }

        return posIndex;
    }
}
