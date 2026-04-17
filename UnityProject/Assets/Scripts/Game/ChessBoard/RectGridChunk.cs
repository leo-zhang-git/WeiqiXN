using System.Collections.Generic;
using UnityEngine;
using XNClient.Logger;

namespace XNClient.ChessBoard
{
    public class RectGridChunk : MonoBehaviour
    {
        public int startCellX, startCellZ;
        public int chunkSizeX, chunkSizeZ;
        public RectMesh groundMesh, roadMesh;

        private List<RectCell> cellList = new List<RectCell>();
        private bool isDirty;

        private void LateUpdate()
        {
            if (isDirty) {
                TriangulateChunk();
                isDirty = false;
            }
        }

        public void InitChunk(int startCellX, int startCellZ, int chunkSizeX, int chunkSizeZ)
        {
            if (startCellX < 0 || startCellZ < 0) {
                XNLogger.LogError("Chunk start cell should be positive, init chunk failed.", ("startCellX", startCellX.ToString()), ("startCellZ", startCellZ.ToString()));
                return;
            }
            this.startCellX = startCellX;
            this.startCellZ = startCellZ;

            if (chunkSizeX <= 0 || chunkSizeZ <= 0) {
                XNLogger.LogError($"Chunk size shoud be positive, init chunk failed.", ("chunkSizeX", chunkSizeX.ToString()), ("chunkSizeZ", chunkSizeZ.ToString()));
                return;
            }
            this.chunkSizeX = chunkSizeX;
            this.chunkSizeZ = chunkSizeZ;

            // 整个棋盘以左下为(0, 0)原点往右上扩张，chunk gameObject的位置为chunk的左下原点
            transform.localPosition = new Vector3(
                startCellX * ChessBoardConfig.rectCellSideLength,
                0f,
                startCellZ * ChessBoardConfig.rectCellSideLength
            );

            cellList.Clear();
        }

        public void AddCellToChunk(RectCell cell)
        {
            if (cell == null || cell.chunk == null || cell.coordinates == null) {
                XNLogger.LogError("Cell, owner or coordinates is null, add cell to chunk failed.");
                return;
            }

            if (cell.chunk != this) {
                XNLogger.LogError(
                    "Cell owner does not match chunk, add cell to chunk failed.",
                    ("cellOwner", cell.chunk.name),
                    ("chunkName", name)
                );
                return;
            }

            int cellX = cell.coordinates.x;
            int cellZ = cell.coordinates.z;
            int minX = startCellX;
            int maxX = startCellX + chunkSizeX;
            int minZ = startCellZ;
            int maxZ = startCellZ + chunkSizeZ;

            if (cellX < minX || cellX >= maxX || cellZ < minZ || cellZ >= maxZ) {
                XNLogger.LogError(
                    "Cell coordinates out of chunk range, add cell to chunk failed.",
                    ("cellX", cellX.ToString()),
                    ("cellZ", cellZ.ToString()),
                    ("startCellX", startCellX.ToString()),
                    ("startCellZ", startCellZ.ToString()),
                    ("chunkSizeX", chunkSizeX.ToString()),
                    ("chunkSizeZ", chunkSizeZ.ToString())
                );
                return;
            }

            cellList.Add(cell);
            isDirty = true;
        }

        public void SetDirty()
        {
            isDirty = true;
        }

        private void TriangulateChunk()
        {
            groundMesh.ClearMesh();
            roadMesh.ClearMesh();

            foreach (RectCell cell in cellList) {
                TriangulateCell(cell);
            }
            groundMesh.RefreshMesh();
            roadMesh.RefreshMesh();
        }

        private void TriangulateCell(RectCell cell)
        {
            // 将方格拆分为东南西北四个方向进行构建
            foreach (RectDirection dir in ChessBoardUtils.GetAllRectDirections()) {
                TriangulateCellByDirection(cell, dir);
            }
        }

        private void TriangulateCellByDirection(RectCell cell, RectDirection dir)
        {
            TriangulateCellInner(cell, dir);
            TriangulateCellOuter(cell, dir);
            TriangulateCellRoad(cell, dir);
        }

        // 内部纯色三角
        private void TriangulateCellInner(RectCell cell, RectDirection dir)
        {
            (Vector3, Vector3) edgeCornerOffsets = ChessBoardUtils.GetInnerCornerOffsets(dir);
            groundMesh.AddTriangle(
                cell.centerPosInChunk,
                cell.centerPosInChunk + edgeCornerOffsets.Item1,
                cell.centerPosInChunk + edgeCornerOffsets.Item2
            );
            groundMesh.AddTriangleColor(
                cell.cellColor,
                cell.cellColor,
                cell.cellColor
            );
        }

        // 外侧混色梯形
        private void TriangulateCellOuter(RectCell cell, RectDirection dir)
        {
            (Vector3, Vector3) innerCornerOffsets = ChessBoardUtils.GetInnerCornerOffsets(dir);
            (Vector3, Vector3) blendCornerOffsets = ChessBoardUtils.GetBlendCornerOffsets(dir);
            (Vector3, Vector3) outerCornerOffsets = ChessBoardUtils.GetOuterCornerOffsets(dir);
            Vector3 outerCorner1 = cell.centerPosInChunk + outerCornerOffsets.Item1;
            Vector3 outerCorner2 = cell.centerPosInChunk + outerCornerOffsets.Item2;
            Vector3 blendCorner1 = cell.centerPosInChunk + blendCornerOffsets.Item1;
            Vector3 blendCorner2 = cell.centerPosInChunk + blendCornerOffsets.Item2;

            // 共用边中点处为两相邻方格颜色，角点为4相邻方格颜色
            Color lineMidColor = cell.GetLineNeighborBlendColor(dir);
            Color pointColor1 = cell.GetPointNeighborBlendColor(dir);
            Color pointColor2 = cell.GetPointNeighborBlendColor(dir.GetNextDirection());
            Color edgeColor1 = Color.Lerp(lineMidColor, pointColor1, ChessBoardConfig.blendFactor);
            Color edgeColor2 = Color.Lerp(lineMidColor, pointColor2, ChessBoardConfig.blendFactor);

            // 中部过渡矩形
            groundMesh.AddQuad(
                cell.centerPosInChunk + innerCornerOffsets.Item1,
                cell.centerPosInChunk + innerCornerOffsets.Item2,
                blendCorner1,
                blendCorner2
            );
            groundMesh.AddQuadColor(
                cell.cellColor,
                cell.cellColor,
                edgeColor1,
                edgeColor2
            );

            // 左右两侧三角
            groundMesh.AddTriangle(
                cell.centerPosInChunk + innerCornerOffsets.Item1,
                outerCorner1,
                blendCorner1
            );
            groundMesh.AddTriangleColor(
                cell.cellColor,
                pointColor1,
                edgeColor1
            );

            groundMesh.AddTriangle(
                cell.centerPosInChunk + innerCornerOffsets.Item2,
                blendCorner2,
                outerCorner2
            );
            groundMesh.AddTriangleColor(
                cell.cellColor,
                edgeColor2,
                pointColor2
            );
        }

        private void TriangulateCellRoad(RectCell cell, RectDirection dir)
        {
            // 道路内侧小三角
            (Vector3, Vector3) centerOffset = ChessBoardUtils.GetRoadCenterCornerOffsets(dir, cell.isOnEdge);
            roadMesh.AddTriangle(
                cell.centerPosInChunk,
                cell.centerPosInChunk + centerOffset.Item1,
                cell.centerPosInChunk + centerOffset.Item2
            );
            roadMesh.AddTriangleColor(
                ChessBoardConfig.roadColor,
                ChessBoardConfig.roadColor,
                ChessBoardConfig.roadColor
            );

            // 道路主体矩形
            if (cell.TryGetLineNeighbor(dir, out RectCell neighbor)) {
                (Vector3, Vector3) innerOffset = ChessBoardUtils.GetRoadInnerCornerOffsets(dir, cell.isOnEdge, neighbor.isOnEdge);
                (Vector3, Vector3) outerOffset = ChessBoardUtils.GetRoadOuterCornerOffsets(dir, cell.isOnEdge, neighbor.isOnEdge);
                roadMesh.AddQuad(
                    cell.centerPosInChunk + innerOffset.Item1,
                    cell.centerPosInChunk + innerOffset.Item2,
                    cell.centerPosInChunk + outerOffset.Item1,
                    cell.centerPosInChunk + outerOffset.Item2

                );
                roadMesh.AddQuadColor(ChessBoardConfig.roadColor);
            }
        }

        public string GetDebugCellLayout()
        {
            RectCell[,] cellGrid = new RectCell[chunkSizeX, chunkSizeZ];
            foreach (RectCell cell in cellList) {
                if (cell?.coordinates == null) {
                    continue;
                }

                int localX = cell.coordinates.x - startCellX;
                int localZ = cell.coordinates.z - startCellZ;
                if (localX < 0 || localX >= chunkSizeX || localZ < 0 || localZ >= chunkSizeZ) {
                    continue;
                }

                cellGrid[localX, localZ] = cell;
            }

            var sb = new System.Text.StringBuilder();
            for (int z = chunkSizeZ - 1; z >= 0; z--) {
                for (int x = 0; x < chunkSizeX; x++) {
                    if (x > 0) {
                        sb.Append(" | ");
                    }

                    RectCell cell = cellGrid[x, z];
                    if (cell?.coordinates == null) {
                        sb.Append("(null)");
                        continue;
                    }

                    sb.Append($"({x},{z})");
                }

                if (z > 0) {
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        [ContextMenu("Debug Print All Cells")]
        public void DebugPrintAllCells()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($"RectGridChunk cells count: {cellList.Count}");
            sb.AppendLine();
            sb.Append(GetDebugCellLayout());

            XNLogger.LogInfo(
                sb.ToString(),
                ("startCellX", startCellX.ToString()),
                ("startCellZ", startCellZ.ToString()),
                ("chunkSizeX", chunkSizeX.ToString()),
                ("chunkSizeZ", chunkSizeZ.ToString())
            );
        }
    }

}
