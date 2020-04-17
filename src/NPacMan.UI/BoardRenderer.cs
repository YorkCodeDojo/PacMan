using System;
using System.Drawing;
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
            int NumberOfCellsWide = 27;
            var cellSize = totalClientWidth / NumberOfCellsWide;

            var walls = game.Walls;

            foreach (var wall in walls)
            {
                var x = wall.x * cellSize;
                var y = wall.y * cellSize;

                g.FillRectangle(Brushes.Blue, x, y, cellSize, cellSize);
            }
        }

        public void RenderCoins(Graphics g, int totalClientWidth, int totalClientHeight, NPacMan.Game.Game game)
        {
            int NumberOfCellsWide = 28;
            var cellSize = totalClientWidth / NumberOfCellsWide;

            var coins = game.Coins;

            foreach (var coin in coins)
            {
                var x = coin.x * cellSize;
                var y = coin.y * cellSize;

                g.FillEllipse(Brushes.Gold, x + (cellSize / 2), y + (cellSize / 2), cellSize / 2, cellSize / 2);
            }
        }

        public void RenderPacMan(Graphics g, int totalClientWidth, int totalClientHeight, NPacMan.Game.Game game)
        {
            int NumberOfCellsWide = 27;
            var cellSize = totalClientWidth / NumberOfCellsWide;

            var x = game.PacMan.X * cellSize;
            var y = game.PacMan.Y * cellSize;

            var offset = 0;

            switch  (game.PacMan.Direction)
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

            var halfSize = (mouthSize /2 );
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
