using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XNClient.Logger;

namespace XNClient.ChessBoard
{
    public class RectGrid : MonoBehaviour
    {
        public GameObject chunkPrefab;
        private List<RectGridChunk> chunkList = new List<RectGridChunk>();
        private List<RectCell> cellList = new List<RectCell>();

        private void Start()
        {
            InitGrid();
        }

        public void InitGrid()
        {
            if (chunkPrefab == null || chunkPrefab.GetComponent<RectGridChunk>() == null) {
                XNLogger.LogError("Chunk prefab invalid, init grid failed.");
                return;
            }

            CreateChunks();
            CreateCells();
        }

        private void CreateChunks()
        {
            int gridSize = ChessBoardConfig.defaultGridSize;
            int chunkSize = ChessBoardConfig.defaultChunkSize;

            chunkList.Clear();
            for (int startCellZ = 0; startCellZ < gridSize; startCellZ += chunkSize) {
                int curChunkSizeZ = Mathf.Min(chunkSize, gridSize - startCellZ);

                for (int startCellX = 0; startCellX < gridSize; startCellX += chunkSize) {
                    int curChunkSizeX = Mathf.Min(chunkSize, gridSize - startCellX);
                    GameObject chunkGO = Instantiate(chunkPrefab, transform);
                    chunkGO.name = $"RectGridChunk_{startCellX}_{startCellZ}";

                    RectGridChunk chunk = chunkGO.GetComponent<RectGridChunk>();
                    chunk.InitChunk(startCellX, startCellZ, curChunkSizeX, curChunkSizeZ);
                    chunkList.Add(chunk);

                    chunk.SetDirty();
                }
            }
        }

        private void CreateCells()
        {
            int gridSize = ChessBoardConfig.defaultGridSize;
            int chunkSize = ChessBoardConfig.defaultChunkSize;
            int chunkCountX = Mathf.CeilToInt((float)gridSize / chunkSize);

            cellList.Clear();
            for (int cellZ = 0; cellZ < gridSize; cellZ++) {
                for (int cellX = 0; cellX < gridSize; cellX++) {
                    int chunkX = cellX / chunkSize;
                    int chunkZ = cellZ / chunkSize;
                    int chunkIndex = chunkZ * chunkCountX + chunkX;

                    if (chunkIndex < 0 || chunkIndex >= chunkList.Count) {
                        XNLogger.LogError(
                            "Chunk index out of range, add cell to chunk failed.",
                            ("cellX", cellX.ToString()),
                            ("cellZ", cellZ.ToString()),
                            ("chunkIndex", chunkIndex.ToString()),
                            ("chunkCount", chunkList.Count.ToString())
                        );
                        continue;
                    }

                    RectGridChunk ownerChunk = chunkList[chunkIndex];
                    RectCell cell = CreateCell(ownerChunk, cellX, cellZ);
                    cellList.Add(cell);
                    chunkList[chunkIndex].AddCellToChunk(cell);
                }
            }
        }

        private RectCell CreateCell(RectGridChunk ownerChunk, int x, int z)
        {
            RectCell cell = new RectCell(ownerChunk, new RectCoordinates(x, z));

            if (x > 0) {
                RectCell westNeighbor = cellList[cellList.Count - 1];
                cell.neighbors[(int)RectDirection.W] = westNeighbor;
                westNeighbor.neighbors[(int)RectDirection.E] = cell;
            }

            if (z > 0) {
                int southNeighborIndex = (z - 1) * ChessBoardConfig.defaultGridSize + x;
                RectCell southNeighbor = cellList[southNeighborIndex];
                cell.neighbors[(int)RectDirection.S] = southNeighbor;
                southNeighbor.neighbors[(int)RectDirection.N] = cell;
            }

            return cell;
        }

        [ContextMenu("Debug Print All Cells")]
        public void DebugPrintAllCells()
        {
            var sb = new StringBuilder();
            sb.Append("RectGrid debug print all cells by chunk order.");

            for (int i = 0; i < chunkList.Count; i++) {
                RectGridChunk chunk = chunkList[i];
                if (chunk == null) {
                    sb.AppendLine();
                    sb.Append($"chunkIndex:{i} null");
                    continue;
                }

                sb.AppendLine();
                sb.Append($"chunkIndex:{i} startCell:({chunk.startCellX},{chunk.startCellZ}) size:({chunk.chunkSizeX},{chunk.chunkSizeZ})");
                sb.AppendLine();
                sb.Append(chunk.GetDebugCellLayout());
            }

            XNLogger.LogInfo(
                sb.ToString(),
                ("gridSize", ChessBoardConfig.defaultGridSize.ToString()),
                ("chunkCount", chunkList.Count.ToString()),
                ("cellCount", cellList.Count.ToString())
            );
        }
    }
}
