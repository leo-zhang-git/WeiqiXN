using System.Numerics;

namespace XNClient.ChessBoard
{
    public class RectCoordinates
    {
        public static Vector3 zeroPos = Vector3.Zero;

        public int x;
        public int z;

        public RectCoordinates(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public Vector3 GetCenterPos()
        {
            float xOffset = (x + 0.5f) * ChessBoardConfig.rectCellSideLength;
            float zOffset = (z + 0.5f) * ChessBoardConfig.rectCellSideLength;
            return zeroPos + new Vector3(xOffset, 0, zOffset);
        }

        public override string ToString()
        {
            return $"(x:{x}, z:{z})";
        }
    }
}
