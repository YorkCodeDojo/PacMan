using NPacMan.Game;
using System.Collections.Generic;
using System.Linq;

namespace NPacMan.UI
{
    public class WallAnalyzer
    {
        public static WallType GetWallType(IReadOnlyCollection<CellLocation> walls, CellLocation wall, int width, int height)
        {
            var wallLeft = walls.Contains((wall.X - 1, wall.Y));
            var wallRight = walls.Contains((wall.X + 1, wall.Y));
            var wallAbove = walls.Contains((wall.X, wall.Y - 1));
            var wallBelow = walls.Contains((wall.X, wall.Y + 1));
            var wallAboveRight = walls.Contains((wall.X + 1, wall.Y - 1));
            var wallAboveLeft = walls.Contains((wall.X - 1, wall.Y - 1));
            var wallBelowLeft = walls.Contains((wall.X - 1, wall.Y + 1));
            var wallBelowRight = walls.Contains((wall.X + 1, wall.Y + 1));
            var topEdge = wall.Y == 0;
            var leftEdge = wall.Y == 0;
            var rightEdge = wall.X + 1 == width;

            if (topEdge && wallLeft && wallRight && wallBelow && !wallBelowLeft)
                return WallType.TopRightArc;

            if (rightEdge && topEdge && wallLeft && wallBelow)
                return WallType.TopRightArc;

            if (leftEdge && topEdge && wallRight && wallBelow)
                return WallType.TopLeftArc;

            if (wallRight && wallBelow && !wallAboveLeft && !wallAbove && !wallLeft)
                return WallType.TopLeftArc;

            if (wallLeft && wallBelow && !wallAboveRight && !wallAbove && !wallRight)
                return WallType.TopRightArc;

            if (wallRight && wallAbove && !wallAboveRight)
                return WallType.BottomLeftArc;

            if (wallRight && wallBelow && !wallBelowRight)
                return WallType.TopLeftArc;

            if (wallLeft && wallBelow && !wallBelowLeft)
                return WallType.TopRightArc;

            if (wallRight && wallAbove && !wallBelowLeft && !wallBelow && !wallLeft)
                return WallType.BottomLeftArc;

            if (wallLeft && wallAbove && !wallAboveLeft)
                return WallType.BottomRightArc;

            if (wallLeft && wallAbove && !wallBelowRight && !wallBelow && !wallRight)
                return WallType.BottomRightArc;

            if (wallAbove && wallBelow)
                return WallType.VerticalLine;
            
            return WallType.HorizontalLine;

        }
    }
}