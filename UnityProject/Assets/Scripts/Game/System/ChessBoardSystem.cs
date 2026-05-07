using System.Collections.Generic;
using UnityEngine;
using XNClient.Logger;

public class ChessBoardSystem : SystemBase
{
    public override string systemName => GetSystemName<ChessBoardSystem>();
    public ChessBoardDataType chessBoardData;

    public ChessBoardSystem(SceneBase scene) : base(scene)
    {

    }

    public override void Init()
    {
        base.Init();

        scene.RegisterSystemEvent<OnAddChessToBoard>(OnAddChessToBoard);

        var compChessBoard = scene.GetComponent<SceneComponentChessBoard>();
        if (compChessBoard == null) return;

        // 非读档进来的需要手动初始化
        if (scene.sceneCreateParams.saveFilePath == null) {
            if (scene.sceneCreateParams.duelSceneCreateParamas != null) {
                compChessBoard.boardCfgId.value = scene.sceneCreateParams.duelSceneCreateParamas.boardCfgId;
            } else {
                XNLogger.LogError("Scene create params for duel scene is empty, init scene with default values.");
                compChessBoard.boardCfgId.value = "9x9";
            }
        }

        chessBoardData = ChessBoardDataType.GetConfigData(compChessBoard.boardCfgId.value);
        if (chessBoardData != null) {
            compChessBoard.chessBoardGrid.InitGrid(chessBoardData.boardSize);
            int chessBoardCellCount = compChessBoard.chessBoardGrid.gridSize * compChessBoard.chessBoardGrid.gridSize;
            compChessBoard.chessFlagMap = new List<int>(new int[chessBoardCellCount]);
            Bounds gridBounds = compChessBoard.chessBoardGrid.GetGridBounds();
            InitDuelVCam(gridBounds);
        } else {
            XNLogger.LogError("Chess board config not found!", ("chessBoardCfgId", compChessBoard.boardCfgId.value));
        }
    }

    public void OnAddChessToBoard(OnAddChessToBoard evt)
    {
        var compDuel = scene.GetComponent<SceneComponentDuel>();
        var compChessBoard = scene.GetComponent<SceneComponentChessBoard>();
        if (compDuel != null && compChessBoard != null) {
            int posIndex = compChessBoard.GetPosIndexByCoords(evt.coords);
            var curPlayer = scene.GetEntity<Player>(compDuel.curTurnPlayerGuid.value);
            if (posIndex >= 0 && compChessBoard.chessFlagMap[posIndex] == 0 && curPlayer != null) {
                compChessBoard.chessFlagMap[posIndex] = curPlayer.playerFlag.value;
                EntityUtils.CreateChess(scene, (PlayerFlag)curPlayer.playerFlag.value, evt.coords);
                scene.EmitSystemEvent(new OnAfterAddChessToBoard());
            }
        }
    }

    private void InitDuelVCam(Bounds gridBound)
    {
        var compChessBoard = scene.GetComponent<SceneComponentChessBoard>();
        if (compChessBoard.duelVCam == null) {
            XNLogger.LogError("Duel virtual camera not found, init camera failed.");
            return;
        }

        Transform duelVCamTransform = compChessBoard.duelVCam.transform;

        // 让相机始终垂直朝向 y 轴负方向，形成俯视棋盘的视角
        duelVCamTransform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);

        float nearClipPlane = compChessBoard.duelVCam.m_Lens.NearClipPlane;
        float halfVerticalFovRad = compChessBoard.duelVCam.m_Lens.FieldOfView * 0.5f * Mathf.Deg2Rad;
        float aspect = Camera.main != null ? Camera.main.aspect : 16f / 9f;

        // 先计算近平面在当前镜头参数下的半高和半宽
        float nearPlaneHalfHeight = Mathf.Tan(halfVerticalFovRad) * nearClipPlane;
        float nearPlaneHalfWidth = nearPlaneHalfHeight * aspect;

        // 根据相似三角形反推相机需要离棋盘中心多高，才能让近平面恰好覆盖 gridBound 的 extent。
        float requiredDistanceByZ = nearPlaneHalfHeight > 0f ? gridBound.extents.z * nearClipPlane / nearPlaneHalfHeight : 0f;
        float requiredDistanceByX = nearPlaneHalfWidth > 0f ? gridBound.extents.x * nearClipPlane / nearPlaneHalfWidth : 0f;
        float requiredDistance = Mathf.Max(requiredDistanceByX, requiredDistanceByZ, nearClipPlane);

        // 以棋盘中心为基准，只沿 y 轴正方向抬升相机位置
        float extraYOffset = 0;
        if (chessBoardData != null) {
            extraYOffset = chessBoardData.vcamYOffset;
        }
        duelVCamTransform.position = gridBound.center + Vector3.up * requiredDistance + new Vector3(0, extraYOffset, 0);
    }
}
