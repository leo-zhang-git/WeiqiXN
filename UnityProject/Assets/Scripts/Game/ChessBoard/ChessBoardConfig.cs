using UnityEngine;

namespace XNClient.ChessBoard
{
    public enum RectDirection
    {
        E = 0,
        S = 1,
        W = 2,
        N = 3,
    }

    public static class ChessBoardConfig
    {
        public const float rectCellSideLength = 6f; // 方格边长
        private const float sqrt2 = 1.41421356f;
        public const float rectCellOuterRadius = rectCellSideLength / 2f * sqrt2; // 方格外切圆半径（中心到顶点长度）
        public const float shrinkFactor = 0.6f; // 方格内圈收缩因子

        // 正方格右上逆时针到左上四个顶点相对偏移
        public static Vector3[] rectCornerOffsets =
        {
            new Vector3(rectCellSideLength / 2f, 0, rectCellSideLength / 2f),
            new Vector3(rectCellSideLength / 2f, 0, -rectCellSideLength / 2f),
            new Vector3(-rectCellSideLength / 2f, 0, -rectCellSideLength / 2f),
            new Vector3(-rectCellSideLength / 2f, 0, rectCellSideLength / 2f),
            // 增加一份回到原点处理最后一个点的情况
            new Vector3(rectCellSideLength / 2f, 0, rectCellSideLength / 2f)
        };

        public static (Vector3, Vector3) GetInnerCornerOffsets(RectDirection dir)
        {
            return (rectCornerOffsets[(int)dir] * shrinkFactor, rectCornerOffsets[(int)dir + 1] * shrinkFactor);
        }

        public static (Vector3, Vector3) GetOuterCornerOffsets(RectDirection dir)
        {
            return (rectCornerOffsets[(int)dir], rectCornerOffsets[(int)dir + 1]);
        }

        public const int defaultGridSize = 19;
        public const int defaultChunkSize = 5;
    }

}