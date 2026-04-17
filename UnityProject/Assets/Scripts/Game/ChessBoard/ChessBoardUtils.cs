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

        public static (Vector3, Vector3) GetRoadCornerOffsets(RectDirection dir)
        {
            return (ChessBoardConfig.rectCornerOffsets[(int)dir] * ChessBoardConfig.roadFactor, ChessBoardConfig.rectCornerOffsets[(int)dir.GetNextDirection()] * ChessBoardConfig.roadFactor);
        }
    }
}
