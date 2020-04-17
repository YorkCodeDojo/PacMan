using System;
using System.Drawing;
using System.Linq;
using NPacMan.Game;

namespace NPacMan.UI
{
    public class BoardRenderer
    {
        private readonly Font _scoreFont = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

        private int mouthSize = 60;
        private int mouthDirection = 1;

        public void RenderWalls(Graphics g, int totalClientWidth, int totalClientHeight, NPacMan.Game.Game game)
        {
            int NumberOfCellsWide = game.Width;
            var cellSize = totalClientWidth / NumberOfCellsWide;
            var wallWidth = cellSize / 5;
            var wallPen = new Pen(Brushes.Blue, wallWidth);
            var walls = game.Walls;

            foreach (var wall in walls)
            {
                var x = wall.x * cellSize;
                var y = wall.y * cellSize;

                // Is there a space above us?
                var wallAbove = walls.Contains((wall.x, wall.y - 1));
                var wallBelow = walls.Contains((wall.x, wall.y + 1));
                var wallLeft = walls.Contains((wall.x - 1, wall.y));
                var wallRight = walls.Contains((wall.x + 1, wall.y));
                var wallAboveRight = walls.Contains((wall.x + 1, wall.y + 1));
                var wallBelowRight = walls.Contains((wall.x + 1, wall.y - 1));
                var wallAboveLeft = walls.Contains((wall.x - 1, wall.y + 1));
                var wallBelowLeft = walls.Contains((wall.x - 1, wall.y - 1));

                var offset = (cellSize - wallWidth) / 2;

                g.DrawRectangle(Pens.White, x, y, cellSize, cellSize);


                if (wallAbove && wallBelow)
                {
                    // Vertical wall
                    if (!wallLeft)
                    {
                        // Must be coins,  so pad away from the coins
                        g.FillRectangle(Brushes.Blue, x + offset, y, wallWidth, cellSize);
                    }
                    else if (!wallRight)
                    {
                        g.FillRectangle(Brushes.Blue, x + offset, y, wallWidth, cellSize);
                    }
                }
                if (wallLeft && wallRight)
                {
                    if (!wallAbove)
                    {
                        // Must be coins,  so pad away from the coins
                        g.FillRectangle(Brushes.Blue, x, y + offset, cellSize, wallWidth);
                    }
                    else if (!wallBelow)
                    {
                        g.FillRectangle(Brushes.Blue, x, y + offset, cellSize, wallWidth);
                    }
                }
                if (wallRight && wallBelow && !wallLeft && !wallAbove)
                {
                    g.DrawArc(wallPen, x + (cellSize / 2), y + (cellSize / 2), cellSize, cellSize, 180, 90);
                }
                if (wallRight && wallAbove && !wallBelow && !wallLeft)
                {
                    g.DrawArc(wallPen, x + (cellSize / 2), y - (cellSize / 2), cellSize, cellSize, 90, 90);
                }
                if (wallLeft && wallBelow && !wallAbove && !wallRight)
                {
                    g.DrawArc(wallPen, x - (cellSize / 2), y + (cellSize / 2), cellSize, cellSize, 270, 90);
                }
                if (wallAbove && wallLeft && !wallBelow && !wallRight)
                {
                    g.DrawArc(wallPen, x - (cellSize / 2), y - (cellSize / 2), cellSize, cellSize, 0, 90);
                }
                if (wallAbove && wallBelow && wallRight && wallLeft)
                {
                    if (!wallAboveRight)
                    {
                        g.DrawArc(wallPen, x + (cellSize / 2), y + (cellSize / 2), cellSize, cellSize, 180, 90);
                    }
                    if (!wallBelowRight)
                    {
                        g.DrawArc(wallPen, x + (cellSize / 2), y - (cellSize / 2), cellSize, cellSize, 90, 90);
                    }
                    if (!wallAboveLeft)
                    {
                        g.DrawArc(wallPen, x - (cellSize / 2), y + (cellSize / 2), cellSize, cellSize, 270, 90);
                    }

                    if (!wallBelowLeft)
                    {
                        g.DrawArc(wallPen, x - (cellSize / 2), y - (cellSize / 2), cellSize, cellSize, 0, 90);
                    }
                }

            }
        }

        public void RenderCoins(Graphics g, int totalClientWidth, int totalClientHeight, NPacMan.Game.Game game)
        {
            int NumberOfCellsWide = game.Width;
            var cellSize = totalClientWidth / NumberOfCellsWide;

            var coins = game.Coins;

            foreach (var coin in coins)
            {
                var x = coin.x * cellSize;
                var y = coin.y * cellSize;

                g.FillRectangle(Brushes.Gold, x + (cellSize / 4), y + (cellSize / 4), cellSize / 2, cellSize / 2);
            }
        }

        public void RenderPacMan(Graphics g, int totalClientWidth, int totalClientHeight, NPacMan.Game.Game game)
        {
            int NumberOfCellsWide = game.Width;
            var cellSize = totalClientWidth / NumberOfCellsWide;

            var x = game.PacMan.X * cellSize;
            var y = game.PacMan.Y * cellSize;

            var offset = 0;

            switch (game.PacMan.Direction)
            {
                case Direction.Up:
                    offset = -90;
                    break;
                case Direction.Left:
                    offset = -180;
                    break;
                case Direction.Down:
                    offset = 90;
                    break;
            }

            var halfSize = (mouthSize / 2);
            g.FillPie(Brushes.Yellow, x, y, cellSize, cellSize, offset + halfSize, 360 - mouthSize);

            mouthSize = (mouthSize + mouthDirection);
            if (mouthSize >= 60)
            {
                mouthDirection = -5;
            }
            else if (mouthSize <= 0)
            {
                mouthDirection = +5;
            }

        }

        public void RenderScore(Graphics g, int totalClientWidth, int totalClientHeight, NPacMan.Game.Game game)
        {
            g.DrawString($"Score : {game.Score}", _scoreFont, Brushes.White, totalClientWidth - 200, 100);
        }
    }
}
