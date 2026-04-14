using UnityEngine;
using XNClient.Logger;

namespace XNClient.ChessBoard
{
    public class RectGridChunk : MonoBehaviour
    {
        public int chunkOffsetX, chunkOffsetZ;
        public int chunkSizeX, chunkSizeZ;
        public RectMesh ground;
        public bool isDirty;

        private void LateUpdate()
        {
            if (isDirty) {
                Triangulate();
            }
        }

        public void InitChunk(int chunkOffsetX, int chunkOffsetZ, int chunkSizeX, int chunkSizeZ)
        {
            if (chunkOffsetX < 0 || chunkOffsetZ < 0) {
                XNLogger.LogError($"Chunk offset should be postive, init chunk failed.", ("chunkOffsetX", chunkOffsetX.ToString()), ("chunkOffsetZ", chunkOffsetZ.ToString()));
                return;
            }
            this.chunkOffsetX = chunkOffsetX;
            this.chunkOffsetZ = chunkOffsetZ;

            if (chunkSizeX <= 0 || chunkSizeZ <= 0) {
                XNLogger.LogError($"Chunk size shoud be positive, init chunk failed.", ("chunkSizeX", chunkSizeX.ToString()), ("chunkSizeZ", chunkSizeZ.ToString()));
                return;
            }
            this.chunkSizeX = chunkSizeX;
            this.chunkSizeZ = chunkSizeZ;
        }

        public void CreateCells()
        {

        }

        public void CreateCell(int x, int z)
        {

        }

        public void Triangulate()
        {

        }
    }

}
