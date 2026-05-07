using System;
using UnityEngine;
using XNClient.ChessBoard;

public static class EntityUtils
{
    private static long _timeStamp = 0;
    private static int _guidInc = 0;
    public static string CreateGuidWithEntityType(string entityType)
    {
        long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (timeStamp > _timeStamp) {
            _timeStamp = timeStamp;
            _guidInc = 0;
        } else {
            _guidInc += 1;
        }
        return $"{entityType}_{timeStamp}_{_guidInc}";
    }

    public static Player CreatePlayer(SceneBase scene, PlayerFlag playerFlag)
    {
        string guid = CreateGuidWithEntityType(EntityBase.GetEntityType<Player>());
        Player player = new Player(scene, guid, playerFlag);
        scene.AddEntity(player);

        return player;
    }

    public static void CreateChess(SceneBase scene, PlayerFlag playerFlag, RectCoordinates coords)
    {
        string gamePrefabTypeId = DuelUtils.GetGamePrefabTypeIdWithPlayerFlag(playerFlag);
        var gamePrefabCfg = GamePrefabDataType.GetConfigData(gamePrefabTypeId);
        if (gamePrefabCfg != null) {
            Global.Instance.resourceManager.LoadGamePrefabAsync(scene, gamePrefabCfg.resPath, (GameObject go) =>
            {
                string guid = CreateGuidWithEntityType(EntityBase.GetEntityType<Chess>());
                Chess chess = new Chess(scene, guid, go, playerFlag, coords);
                scene.AddEntity(chess);
            });
        }
    }
}
