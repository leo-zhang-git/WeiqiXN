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

        // 定义cell中心颜色为(1, 0, 0, 0)，定义每个角的三个相邻cell的颜色，后续用来叠加uv采样出的贴图颜色
        private static Color color1 = new Color(1f, 0f, 0f, 0f);
        private static Color color2 = new Color(0f, 1f, 0f, 0f);
        private static Color color3 = new Color(0f, 0f, 1f, 0f);
        private static Color color4 = new Color(0f, 0f, 0f, 1f);

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
            groundMesh.AddTriangleColor(color1);
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

            Color cellColor = color1;
            Color lineMidColor = GetRelativeLineMidColor(cell, dir);
            Color pointColor1 = GetRelativeOuterPointColor1(cell, dir);
            Color pointColor2 = GetRelativeOuterPointColor2(cell, dir);
            Color edgeLerpColor1 = Color.Lerp(pointColor1, lineMidColor, ChessBoardConfig.blendFactor);
            Color edgeLerpColor2 = Color.Lerp(pointColor2, lineMidColor, ChessBoardConfig.blendFactor);

            // 中部过渡矩形
            groundMesh.AddQuad(
                cell.centerPosInChunk + innerCornerOffsets.Item1,
                cell.centerPosInChunk + innerCornerOffsets.Item2,
                blendCorner1,
                blendCorner2
            );
            groundMesh.AddQuadColor(
                cellColor,
                cellColor,
                edgeLerpColor1,
                edgeLerpColor2
            );

            // 左右两侧三角
            groundMesh.AddTriangle(
                cell.centerPosInChunk + innerCornerOffsets.Item1,
                outerCorner1,
                blendCorner1
            );
            groundMesh.AddTriangleColor(
                cellColor,
                pointColor1,
                edgeLerpColor1
            );

            groundMesh.AddTriangle(
                cell.centerPosInChunk + innerCornerOffsets.Item2,
                blendCorner2,
                outerCorner2
            );
            groundMesh.AddTriangleColor(
                cellColor,
                edgeLerpColor2,
                pointColor2
            );
        }

        // 获取当前边中点的相对颜色，仅由自身和该边相邻方格共同决定
        private Color GetRelativeLineMidColor(RectCell cell, RectDirection dir)
        {
            Color relativeColor = color1;
            float colorCount = 1f;
            if (cell.TryGetLineNeighbor(dir, out _)) {
                relativeColor += color2;
                colorCount += 1f;
            }

            return relativeColor / colorCount;
        }

        // 获取当前方向第一个外角的相对颜色，按 prevDir -> point(dir) -> dir 的邻域顺序累加
        private Color GetRelativeOuterPointColor1(RectCell cell, RectDirection dir)
        {
            RectDirection prevDir = dir.GetPrevDirection();
            Color relativeColor = color1;
            float colorCount = 1f;

            if (cell.TryGetLineNeighbor(prevDir, out _)) {
                relativeColor += color4;
                colorCount += 1f;
            }
            if (cell.TryGetPointNeighbor(dir, out _)) {
                relativeColor += color3;
                colorCount += 1f;
            }
            if (cell.TryGetLineNeighbor(dir, out _)) {
                relativeColor += color2;
                colorCount += 1f;
            }

            return relativeColor / colorCount;
        }

        // 获取当前方向第二个外角的相对颜色，按 dir -> point(nextDir) -> nextDir 的邻域顺序累加
        private Color GetRelativeOuterPointColor2(RectCell cell, RectDirection dir)
        {
            Color relativeColor = color1;
            float colorCount = 1f;
            RectDirection nextDir = dir.GetNextDirection();

            if (cell.TryGetLineNeighbor(dir, out _)) {
                relativeColor += color2;
                colorCount += 1f;
            }
            if (cell.TryGetPointNeighbor(nextDir, out _)) {
                relativeColor += color3;
                colorCount += 1f;
            }
            if (cell.TryGetLineNeighbor(nextDir, out _)) {
                relativeColor += color4;
                colorCount += 1f;
            }

            return relativeColor / colorCount;
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
