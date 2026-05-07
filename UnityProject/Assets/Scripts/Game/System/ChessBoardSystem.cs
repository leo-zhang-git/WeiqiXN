using System;
using System.Collections.Generic;
using UnityEngine;
using XNClient.ChessBoard;
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

        scene.RegisterEntityEvent<Chess, OnEntityCreated>(OnChessCreated);
        scene.RegisterSystemEvent<OnAddChessToBoard>(OnAddChessToBoard);
        scene.RegisterSystemEvent<OnAfterAddChessToBoard>(OnAfterAddChessToBoard);

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
            compChessBoard.chessInfoMap = new ChessInfo[chessBoardCellCount];
            Bounds gridBounds = compChessBoard.chessBoardGrid.GetGridBounds();
            InitDuelVCam(gridBounds);
        } else {
            XNLogger.LogError("Chess board config not found!", ("chessBoardCfgId", compChessBoard.boardCfgId.value));
        }
    }

    public void OnChessCreated(Chess chess, OnEntityCreated evt)
    {
        var compChessBoard = scene.GetComponent<SceneComponentChessBoard>();
        if (compChessBoard != null) {
            int posIndex = compChessBoard.GetPosIndexByCoords(chess.coords);
            if (posIndex >= 0) {
                var chessInfo = compChessBoard.chessInfoMap[posIndex];
                if (chessInfo.chessGuid == chess.guid && chessInfo.chessFlag == (int)chess.playerFlag) {
                    Transform gridTransform = compChessBoard.chessBoardGrid.transform;
                    Vector3 localChessPos = new Vector3(
                        (chess.coords.x + 0.5f) * ChessBoardConfig.rectCellSideLength,
                        0f,
                        (chess.coords.z + 0.5f) * ChessBoardConfig.rectCellSideLength
                    );
                    chess.transform.position = gridTransform.TransformPoint(localChessPos);
                } else {
                    chess.Destroy();
                }
            }
        }
    }

    public void OnAddChessToBoard(OnAddChessToBoard evt)
    {
        var compDuel = scene.GetComponent<SceneComponentDuel>();
        var compChessBoard = scene.GetComponent<SceneComponentChessBoard>();
        if (compDuel != null && compChessBoard != null) {
            int posIndex = compChessBoard.GetPosIndexByCoords(evt.coords);
            var curPlayer = scene.GetEntity<Player>(compDuel.curTurnPlayerGuid.value);
            if (posIndex >= 0 && compChessBoard.chessInfoMap[posIndex].chessFlag == 0 && curPlayer != null) {
                string guid = EntityUtils.CreateGuidWithEntityType(EntityBase.GetEntityType<Chess>());
                compChessBoard.chessInfoMap[posIndex].chessGuid = guid;
                compChessBoard.chessInfoMap[posIndex].chessFlag = curPlayer.playerFlag.value;
                EntityUtils.CreateChess(scene, guid, (PlayerFlag)curPlayer.playerFlag.value, evt.coords);
                scene.EmitSystemEvent(new OnAfterAddChessToBoard((PlayerFlag)curPlayer.playerFlag.value, evt.coords.Clone()));
            }
        }
    }

    public void OnAfterAddChessToBoard(OnAfterAddChessToBoard evt)
    {
        List<int> pendingRemovePosIndexes = GetPendingRemovePosIndexes(evt.playerFlag, evt.coords);
        var compChessBoard = scene.GetComponent<SceneComponentChessBoard>();
        if (compChessBoard != null) {
            foreach (var posIndex in pendingRemovePosIndexes) {
                var chess = scene.GetEntity<Chess>(compChessBoard.chessInfoMap[posIndex].chessGuid);
                if (chess != null) {
                    chess.Destroy();
                }
                compChessBoard.chessInfoMap[posIndex].Clear();
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

    private static int[] dirX = { 0, 0, 1, -1 };
    private static int[] dirZ = { 1, -1, 0, 0 };
    private bool[] visited;
    // 新增棋子时，BFS遍历失去所有气的棋子串
    private List<int> GetPendingRemovePosIndexes(PlayerFlag playerFlag, RectCoordinates coords)
    {
        List<int> pendingRemovePosIndexes = new List<int>();
        var compChessBoard = scene.GetComponent<SceneComponentChessBoard>();
        if (compChessBoard != null) {
            visited = new bool[compChessBoard.chessInfoMap.Length];
            for (int dir = 0; dir < Math.Min(dirX.Length, dirZ.Length); dir++) {
                int nx = coords.x + dirX[dir];
                int nz = coords.z + dirZ[dir];
                int nPosIndex = compChessBoard.GetPosIndexByCoords(new RectCoordinates(nx, nz));

                if (nPosIndex < 0 || visited[nPosIndex]) {
                    continue;
                }
                PlayerFlag targetPlayerFlag = playerFlag == PlayerFlag.Player1 ? PlayerFlag.Player2 : PlayerFlag.Player1;
                List<int> connectGroup = GetConnectGroup(nPosIndex, targetPlayerFlag);
                if (!CheckGroupHasLiberty(connectGroup)) {
                    foreach (int _posIndex in connectGroup) {
                        pendingRemovePosIndexes.Add(_posIndex);
                    }
                }
            }
        }
        return pendingRemovePosIndexes;
    }

    // 计算连通图
    private List<int> GetConnectGroup(int startIndex, PlayerFlag targetPlayerFlag)
    {
        List<int> connectGroup = new List<int>();
        var compChessBoard = scene.GetComponent<SceneComponentChessBoard>();
        if (compChessBoard != null) {
            if (startIndex < 0 || startIndex >= compChessBoard.chessInfoMap.Length) {
                return new List<int>();
            }

            if (compChessBoard.chessInfoMap[startIndex].chessFlag != (int)targetPlayerFlag) {
                return new List<int>();
            }

            int gridSize = compChessBoard.chessBoardGrid.gridSize;
            Queue<int> bfsQueue = new Queue<int>();

            bfsQueue.Enqueue(startIndex);
            visited[startIndex] = true;
            connectGroup.Add(startIndex);

            while (bfsQueue.Count > 0) {
                int curIndex = bfsQueue.Dequeue();
                int curX = curIndex % gridSize;
                int curZ = curIndex / gridSize;

                for (int dir = 0; dir < Math.Min(dirX.Length, dirZ.Length); dir++) {
                    int nx = curX + dirX[dir];
                    int nz = curZ + dirZ[dir];
                    int nextIndex = compChessBoard.GetPosIndexByCoords(new RectCoordinates(nx, nz));
                    if (nextIndex < 0 || visited[nextIndex]) {
                        continue;
                    }

                    ChessInfo nextChessInfo = compChessBoard.chessInfoMap[nextIndex];
                    if (nextChessInfo.chessFlag != (int)targetPlayerFlag) {
                        continue;
                    }

                    bfsQueue.Enqueue(nextIndex);
                    visited[nextIndex] = true;
                    connectGroup.Add(nextIndex);
                }
            }
        }

        return connectGroup;
    }

    // 检查连通图是否有气
    private bool CheckGroupHasLiberty(List<int> connectGroup)
    {
        var compChessBoard = scene.GetComponent<SceneComponentChessBoard>();
        if (compChessBoard != null) {
            int gridSize = compChessBoard.chessBoardGrid.gridSize;
            foreach (int posIndex in connectGroup) {
                if (posIndex < 0 || posIndex >= compChessBoard.chessInfoMap.Length) {
                    continue;
                }

                int curX = posIndex % gridSize;
                int curZ = posIndex / gridSize;
                for (int dir = 0; dir < Math.Min(dirX.Length, dirZ.Length); dir++) {
                    int nx = curX + dirX[dir];
                    int nz = curZ + dirZ[dir];
                    int neighborIndex = compChessBoard.GetPosIndexByCoords(new RectCoordinates(nx, nz));
                    if (neighborIndex < 0) {
                        continue;
                    }

                    ChessInfo neighborChessInfo = compChessBoard.chessInfoMap[neighborIndex];
                    if (neighborChessInfo.chessFlag == 0) {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
