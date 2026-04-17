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
        public const int chessBoardChunkSize = 5; // 棋盘grid分块大小，主要影响mesh数量
        public const float rectCellSideLength = 6f; // 方格边长
        private const float sqrt_2 = 1.41421356f;
        public const float rectCellOuterRadius = rectCellSideLength / 2f * sqrt_2; // 方格外切圆半径（中心到顶点长度）
        public const float shrinkFactor = 0.6f; // 方格内圈收缩部分比例
        public const float blendFactor = 1 - shrinkFactor; // 方格外圈过渡部分比例
        public static readonly Color roadColor = new Color(0.12f, 0.12f, 0.12f, 1); // 方格内道路颜色
        public const float roadFactor = 0.1f; // 道路从中心扩散处的内侧小三角比例

        // 正方格四个顶点相对偏移，按右上 -> 右下 -> 左下 -> 左上的顺时针顺序存储
        public static Vector3[] rectCornerOffsets =
        {
            new Vector3(rectCellSideLength / 2f, 0, rectCellSideLength / 2f),
            new Vector3(rectCellSideLength / 2f, 0, -rectCellSideLength / 2f),
            new Vector3(-rectCellSideLength / 2f, 0, -rectCellSideLength / 2f),
            new Vector3(-rectCellSideLength / 2f, 0, rectCellSideLength / 2f),
        };
    }
}