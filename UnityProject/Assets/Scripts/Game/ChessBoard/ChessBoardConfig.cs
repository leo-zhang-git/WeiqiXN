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
        public const float shrinkFactor = 0.6f; // 方格内圈收缩部分比例
        public const float blendFactor = 1 - shrinkFactor; // 方格外圈过渡部分比例

        // 正方格四个顶点相对偏移，按右上 -> 右下 -> 左下 -> 左上的顺时针顺序存储
        public static Vector3[] rectCornerOffsets =
        {
            new Vector3(rectCellSideLength / 2f, 0, rectCellSideLength / 2f),
            new Vector3(rectCellSideLength / 2f, 0, -rectCellSideLength / 2f),
            new Vector3(-rectCellSideLength / 2f, 0, -rectCellSideLength / 2f),
            new Vector3(-rectCellSideLength / 2f, 0, rectCellSideLength / 2f),
            // 增加一份回到原点处理最后一个点的情况
            new Vector3(rectCellSideLength / 2f, 0, rectCellSideLength / 2f)
        };

        public static RectDirection GetPrevDirection(this RectDirection dir)
        {
            if (dir == RectDirection.E) {
                return RectDirection.N;
            } else {
                return (RectDirection)((int)dir - 1);
            }
        }

        public static RectDirection GetNextDirection(this RectDirection dir)
        {
            if (dir == RectDirection.N) {
                return RectDirection.E;
            } else {
                return (RectDirection)((int)dir + 1);
            }
        }

        public static (Vector3, Vector3) GetInnerCornerOffsets(RectDirection dir)
        {
            return (rectCornerOffsets[(int)dir] * shrinkFactor, rectCornerOffsets[(int)dir.GetNextDirection()] * shrinkFactor);
        }

        public static (Vector3, Vector3) GetOuterCornerOffsets(RectDirection dir)
        {
            return (rectCornerOffsets[(int)dir], rectCornerOffsets[(int)dir.GetNextDirection()]);
        }

        public static (Vector3, Vector3) GetBlendCornerOffsets(RectDirection dir)
        {
            Vector3 midDir = ((rectCornerOffsets[(int)dir] + rectCornerOffsets[(int)dir.GetNextDirection()]) / 2f).normalized;
            var innerCornerOffstes = GetInnerCornerOffsets(dir);
            float blendWidth = rectCellSideLength / 2f * blendFactor;
            return (innerCornerOffstes.Item1 + midDir * blendWidth, innerCornerOffstes.Item2 + midDir * blendWidth);
        }

        public const int defaultGridSize = 19;
        public const int defaultChunkSize = 5;
    }

}