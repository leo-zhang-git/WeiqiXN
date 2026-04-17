using System.Collections.Generic;
using UnityEngine;

namespace XNClient.ChessBoard
{
    public static class ChessBoardUtils
    {
        private static readonly RectDirection[] allRectDirections =
        {
            RectDirection.E,
            RectDirection.S,
            RectDirection.W,
            RectDirection.N,
        };

        public static IEnumerable<RectDirection> GetAllRectDirections()
        {
            return allRectDirections;
        }

        public static RectDirection GetPrevDirection(this RectDirection dir)
        {
            if (dir == RectDirection.E) {
                return RectDirection.N;
            } else {
                return (RectDirection)(int)(dir - 1);
            }
        }

        public static RectDirection GetNextDirection(this RectDirection dir)
        {
            if (dir == RectDirection.N) {
                return RectDirection.E;
            } else {
                return (RectDirection)(int)(dir + 1);
            }
        }

        public static (Vector3, Vector3) GetInnerCornerOffsets(RectDirection dir)
        {
            return (ChessBoardConfig.rectCornerOffsets[(int)dir] * ChessBoardConfig.shrinkFactor, ChessBoardConfig.rectCornerOffsets[(int)dir.GetNextDirection()] * ChessBoardConfig.shrinkFactor);
        }

        public static (Vector3, Vector3) GetOuterCornerOffsets(RectDirection dir)
        {
            return (ChessBoardConfig.rectCornerOffsets[(int)dir], ChessBoardConfig.rectCornerOffsets[(int)dir.GetNextDirection()]);
        }

        public static (Vector3, Vector3) GetBlendCornerOffsets(RectDirection dir)
        {
            Vector3 midDir = ((ChessBoardConfig.rectCornerOffsets[(int)dir] + ChessBoardConfig.rectCornerOffsets[(int)dir.GetNextDirection()]) / 2f).normalized;
            var innerCornerOffstes = GetInnerCornerOffsets(dir);
            float blendWidth = ChessBoardConfig.rectCellSideLength / 2f * ChessBoardConfig.blendFactor;
            return (innerCornerOffstes.Item1 + midDir * blendWidth, innerCornerOffstes.Item2 + midDir * blendWidth);
        }

        public static (Vector3, Vector3) GetRoadCenterCornerOffsets(RectDirection dir, bool isOnEdge)
        {
            float factor = isOnEdge ? ChessBoardConfig.roadBoderFactor : ChessBoardConfig.roadNormalFactor;
            return (ChessBoardConfig.rectCornerOffsets[(int)dir] * factor, ChessBoardConfig.rectCornerOffsets[(int)dir.GetNextDirection()] * factor);
        }

        public static (Vector3, Vector3) GetRoadInnerCornerOffsets(RectDirection dir, bool isOnEdge, bool isNeighborOnEdge)
        {
            float factor = isOnEdge && isNeighborOnEdge ? ChessBoardConfig.roadBoderFactor : ChessBoardConfig.roadNormalFactor;
            return (ChessBoardConfig.rectCornerOffsets[(int)dir] * factor, ChessBoardConfig.rectCornerOffsets[(int)dir.GetNextDirection()] * factor);

        }

        public static (Vector3, Vector3) GetRoadOuterCornerOffsets(RectDirection dir, bool isOnEdge, bool isNeighborOnEdge)
        {
            (Vector3, Vector3) innerOffset = GetRoadInnerCornerOffsets(dir, isOnEdge, isNeighborOnEdge);
            float factor = isOnEdge && isNeighborOnEdge ? 1 - ChessBoardConfig.roadBoderFactor : 1 - ChessBoardConfig.roadNormalFactor;
            Vector3 midDir = ((innerOffset.Item1 + innerOffset.Item2) / 2f).normalized;
            return (innerOffset.Item1 + midDir * (ChessBoardConfig.rectCellSideLength / 2f * factor), innerOffset.Item2 + midDir * (ChessBoardConfig.rectCellSideLength / 2f * factor));
        }
    }
}
