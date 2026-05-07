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
    }
}
