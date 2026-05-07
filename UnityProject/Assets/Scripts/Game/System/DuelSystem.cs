using UnityEngine;
using XNClient.ChessBoard;

public class DuelSystem : SystemBase
{
    public override string systemName => GetSystemName<DuelSystem>();

    public GameObject aimVFX;
    private const string AIM_VFX_GAME_PREFAB_TYPEID = "FX_LootDrop_Blue";

    public DuelSystem(DuelScene scene) : base(scene)
    {

    }

    public override void Init()
    {
        base.Init();

        // 非读档进来的需要手动初始化
        if (scene.sceneCreateParams.saveFilePath == null) {
            var compDuel = scene.GetComponent<SceneComponentDuel>();
            if (compDuel != null) {
                Player player1 = EntityUtils.CreatePlayer(scene, PlayerFlag.Player1);
                compDuel.player1Guid.value = player1.guid;
                Player player2 = EntityUtils.CreatePlayer(scene, PlayerFlag.Player2);
                compDuel.player2Guid.value = player2.guid;
                compDuel.curTurnPlayerGuid.value = player1.guid;

                compDuel.duelFSM.Activate();
            }
        } else {
            // TODO restore duelFSM
        }

        if (aimVFX == null) {
            var gamePrefabCfg = GamePrefabDataType.GetConfigData(AIM_VFX_GAME_PREFAB_TYPEID);
            if (gamePrefabCfg != null) {
                aimVFX = Global.Instance.resourceManager.LoadGamePrefab(gamePrefabCfg.resPath);
            }
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        var compDuel = scene.GetComponent<SceneComponentDuel>();
        if (compDuel != null) {
            if (compDuel.duelFSM.isActivated) {
                compDuel.duelFSM.Update();
            }

            if (aimVFX != null) {
                if (compDuel.duelFSM.curState.stateName == DuelStateDefine.STATE_TURN_INPUT) {
                    Ray mouseRay = Global.Instance.uiManager.uiCamera.ScreenPointToRay(Input.mousePosition);
                    // UI射线把落子vfx放到指定位置
                    if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out var hitInfo, 500)) {
                        aimVFX.SetActive(true);
                        var compChessBoard = scene.GetComponent<SceneComponentChessBoard>();
                        if (compChessBoard != null) {
                            Transform gridTransform = compChessBoard.chessBoardGrid.transform;
                            Vector3 localHitPoint = gridTransform.InverseTransformPoint(hitInfo.point);
                            float cellSideLength = ChessBoardConfig.rectCellSideLength;

                            int nearestCellX = Mathf.RoundToInt(localHitPoint.x / cellSideLength - 0.5f);
                            int nearestCellZ = Mathf.RoundToInt(localHitPoint.z / cellSideLength - 0.5f);

                            int maxCellIndex = Mathf.Max(compChessBoard.chessBoardGrid.gridSize - 1, 0);
                            nearestCellX = Mathf.Clamp(nearestCellX, 0, maxCellIndex);
                            nearestCellZ = Mathf.Clamp(nearestCellZ, 0, maxCellIndex);

                            Vector3 nearestCellCenterLocalPos = new Vector3(
                                (nearestCellX + 0.5f) * cellSideLength,
                                0f,
                                (nearestCellZ + 0.5f) * cellSideLength
                            );
                            aimVFX.transform.position = gridTransform.TransformPoint(nearestCellCenterLocalPos);
                        }
                    } else {
                        aimVFX.SetActive(false);
                    }
                } else {
                    aimVFX.SetActive(false);
                }
            }
        }
    }
}
