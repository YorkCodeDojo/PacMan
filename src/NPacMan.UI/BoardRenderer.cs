using System;
using System.Drawing;
namespace NPacMan.UI
{
    public class BoardRenderer
    {
        private readonly Font _scoreFont = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

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

                g.FillEllipse(Brushes.Gold, x + (cellSize/2), y + (cellSize / 2), cellSize / 2, cellSize / 2);
            }
        }

        public void RenderPacMan(Graphics g, int totalClientWidth, int totalClientHeight, NPacMan.Game.Game game)
        {
            int NumberOfCellsWide = 27;
            var cellSize = totalClientWidth / NumberOfCellsWide;

            var x = game.PacMan.X * cellSize;
            var y = game.PacMan.Y * cellSize;

            g.FillEllipse(Brushes.Yellow, x, y, cellSize, cellSize);
        }

        public void RenderScore(Graphics g, int totalClientWidth, int totalClientHeight, NPacMan.Game.Game game)
        {
            g.DrawString($"Score : {game.Score}", _scoreFont, Brushes.White, totalClientWidth - 200, 100);
        }
    }
}