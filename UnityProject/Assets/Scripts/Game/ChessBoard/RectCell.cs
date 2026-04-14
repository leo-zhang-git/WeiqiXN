using UnityEngine;

namespace XNClient.ChessBoard
{
    public class RectCell
    {
        public RectGridChunk owner;
        public RectCoordinates coordinates;
        public Vector3 centerPosInChunk
        {
            get
            {
                if (owner == null || coordinates == null) {
                    return Vector3.zero;
                }

                float localX = (coordinates.x - owner.startCellX + 0.5f) * ChessBoardConfig.rectCellSideLength;
                float localZ = (coordinates.z - owner.startCellZ + 0.5f) * ChessBoardConfig.rectCellSideLength;
                return new Vector3(localX, 0f, localZ);
            }
        }

        public RectCell(RectGridChunk owner, RectCoordinates coordinates)
        {
            this.owner = owner;
            this.coordinates = coordinates;
        }
    }

}
