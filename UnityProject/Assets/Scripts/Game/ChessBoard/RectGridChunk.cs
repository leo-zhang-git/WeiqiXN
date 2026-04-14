using System.Collections.Generic;
using UnityEngine;
using XNClient.Logger;

namespace XNClient.ChessBoard
{
    public class RectGridChunk : MonoBehaviour
    {
        public int startCellX, startCellZ;
        public int chunkSizeX, chunkSizeZ;
        public RectMesh ground;

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
            if (cell == null || cell.owner == null || cell.coordinates == null) {
                XNLogger.LogError("Cell, owner or coordinates is null, add cell to chunk failed.");
                return;
            }

            if (cell.owner != this) {
                XNLogger.LogError(
                    "Cell owner does not match chunk, add cell to chunk failed.",
                    ("cellOwner", cell.owner.name),
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
            ground.ClearMesh();

            foreach (RectCell cell in cellList) {
                TriangulateCell(cell);
            }
            ground.RefreshMesh();
        }

        private void TriangulateCell(RectCell cell)
        {
            // 将方格拆分为东南西北四个方向进行构建
            for (RectDirection dir = RectDirection.E; dir <= RectDirection.N; dir++) {
                TriangulateCell(cell, dir);
            }
        }

        private void TriangulateCell(RectCell cell, RectDirection dir)
        {
            (Vector3, Vector3) edgeCornerOffsets = ChessBoardConfig.GetOuterCornerOffsets(dir);
            ground.AddTriangle(
                cell.centerPosInChunk,
                cell.centerPosInChunk + edgeCornerOffsets.Item1,
                cell.centerPosInChunk + edgeCornerOffsets.Item2
            );
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
