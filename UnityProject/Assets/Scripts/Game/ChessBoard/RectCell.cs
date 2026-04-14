using System.Numerics;

namespace XNClient.ChessBoard
{
    public class RectCell
    {
        public RectCoordinates coordinates;
        public Vector3 centerPos
        {
            get
            {
                if (coordinates == null) {
                    return RectCoordinates.zeroPos;
                }
                return coordinates.GetCenterPos();
            }
        }
    }

}