using UnityEngine;
using XNClient.Logger;

public class ChessBoardSystem : SceneSystem<DuelScene>
{
    public override string systemName => GetSystemName<ChessBoardSystem>();
    public ChessBoardDataType chessBoardData;

    public ChessBoardSystem(DuelScene scene) : base(scene)
    {

    }

    public override void Init()
    {
        base.Init();

        chessBoardData = ChessBoardDataType.GetConfigData(scene.compChessBoard.boardCfgId.value);
        if (chessBoardData != null) {
            scene.compChessBoard.chessBoardGrid.InitGrid(chessBoardData.boardSize);
            Bounds gridBounds = scene.compChessBoard.chessBoardGrid.GetGridBounds();
            InitDuelVCam(gridBounds);
        } else {
            XNLogger.LogError("Chess board config not found!", ("chessBoardCfgId", scene.compChessBoard.boardCfgId.value));
        }
    }

    private void InitDuelVCam(Bounds gridBound)
    {
        if (scene.compChessBoard.duelVCam == null) {
            XNLogger.LogError("Duel virtual camera not found, init camera failed.");
            return;
        }

        Transform duelVCamTransform = scene.compChessBoard.duelVCam.transform;

        // 让相机始终垂直朝向 y 轴负方向，形成俯视棋盘的视角
        duelVCamTransform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);

        float nearClipPlane = scene.compChessBoard.duelVCam.m_Lens.NearClipPlane;
        float halfVerticalFovRad = scene.compChessBoard.duelVCam.m_Lens.FieldOfView * 0.5f * Mathf.Deg2Rad;
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
