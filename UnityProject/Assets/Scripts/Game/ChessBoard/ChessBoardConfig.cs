using System.Numerics;

namespace XNClient.ChessBoard
{
    public enum RectDirection
    {
        NE = 0,
        SE = 1,
        SW = 2,
        NW = 4,
    }

    public static class ChessBoardConfig
    {
        public const float rectCellSideLength = 6f; // 正方格边长
        private const float sqrt2 = 1.41421356f;
        public const float rectCellOuterRadius = rectCellSideLength / 2f * sqrt2; // 正方格外切圆半径（中心到顶点长度）

        // 正方格右上逆时针到左上四个顶点相对偏移
        public static Vector3[] rectCornerOffsets =
        {
        new Vector3(rectCellSideLength / 2f, 0, rectCellSideLength / 2f),
        new Vector3(rectCellSideLength / 2f, 0, -rectCellSideLength / 2f),
        new Vector3(-rectCellSideLength / 2f, 0, -rectCellSideLength / 2f),
        new Vector3(-rectCellSideLength / 2f, 0, rectCellSideLength / 2f)
    };
    }

}