using UnityEngine;
using XNClient.ChessBoard;

public class Chess : EntityWithGO
{
    public override string entityType => GetEntityType<Chess>();
    public PlayerFlag playerFlag;
    public RectCoordinates coords;

    public Chess(SceneBase scene, string guid, GameObject gameObject, PlayerFlag playerFlag, RectCoordinates coords) : base(scene, guid, gameObject)
    {
        this.playerFlag = playerFlag;
        this.coords = coords;

        var compChessBoard = scene.GetComponent<SceneComponentChessBoard>();
        if (compChessBoard?.chessBoardGrid != null) {
            Transform gridTransform = compChessBoard.chessBoardGrid.transform;
            Vector3 localChessPos = new Vector3(
                (coords.x + 0.5f) * ChessBoardConfig.rectCellSideLength,
                0f,
                (coords.z + 0.5f) * ChessBoardConfig.rectCellSideLength
            );
            transform.position = gridTransform.TransformPoint(localChessPos);
        }
    }
}
