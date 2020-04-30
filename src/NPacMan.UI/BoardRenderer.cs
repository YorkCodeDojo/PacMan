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

        private bool animated = false;

        private Sprites _sprites;
        private ScoreBoard _scoreBoard;

        public BoardRenderer()
        {
            _sprites = new Sprites();
            _scoreBoard = new ScoreBoard(_sprites);
        }

        public void RenderWalls(Graphics g, NPacMan.Game.Game game)
        {
            var cellSize = Sprites.PixelGrid;
            var wallWidth = cellSize / 5;
            var wallPen = new Pen(Brushes.Blue, wallWidth);
            var walls = game.Walls;

            foreach (var wall in walls)
            {
                var x = wall.x * cellSize;
                var y = wall.y * cellSize;
                var offset = (cellSize - wallWidth) / 2;

               // g.DrawRectangle(Pens.White, x, y, cellSize, cellSize);

                var wallType = WallAnalyzer.GetWallType(walls, wall, game.Width, game.Height);

                switch (wallType)
                {
                    case WallType.VerticalLine:
                        g.FillRectangle(Brushes.Blue, x + offset, y, wallWidth, cellSize);
                        break;
                    case WallType.HorizontalLine:
                        g.FillRectangle(Brushes.Blue, x, y + offset, cellSize, wallWidth);
                        break;
                    case WallType.TopRightArc:
                        g.DrawArc(wallPen, x - (cellSize / 2), y + (cellSize / 2), cellSize, cellSize, 270, 90);
                        break;
                    case WallType.BottomRightArc:
                        g.DrawArc(wallPen, x - (cellSize / 2), y - (cellSize / 2), cellSize, cellSize, 0, 90);
                        break;
                    case WallType.TopLeftArc:
                        g.DrawArc(wallPen, x + (cellSize / 2), y + (cellSize / 2), cellSize, cellSize, 180, 90);
                        break;
                    case WallType.BottomLeftArc:
                        g.DrawArc(wallPen, x + (cellSize / 2), y - (cellSize / 2), cellSize, cellSize, 90, 90);
                        break;
                    default:
                        g.FillRectangle(Brushes.Red, x, y, cellSize, cellSize);
                        break;
                }
            }
        }

        private int CellSizeFromClientSize(Game.Game game, int totalClientWidth, int totalClientHeight)
        {
            return Math.Min(totalClientWidth / game.Width, totalClientHeight / game.Height);
        }

        public void RenderCoins(Graphics g, NPacMan.Game.Game game)
        {
            var cellSize = Sprites.PixelGrid;

            var coins = game.Coins;

            foreach (var coin in coins)
            {
                var x = coin.x * cellSize;
                var y = coin.y * cellSize;

                g.FillRectangle(Brushes.Gold, x + (cellSize / 4), y + (cellSize / 4), cellSize / 2, cellSize / 2);
            }
        }

        public void RenderPacMan(Graphics g, NPacMan.Game.Game game)
        {
            var cellSize = Sprites.PixelGrid;

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

        public void RenderGhosts(Graphics g, NPacMan.Game.Game game)
        {
            var cellSize = Sprites.PixelGrid;

            animated = !animated;

            foreach (var ghost in game.Ghosts.Values)
            {
                RenderGhost(g, cellSize, ghost);
            }
        }
        private void RenderGhost(Graphics g, int cellSize, Ghost ghost)
        {
            var x = ghost.X * cellSize;
            var y = ghost.Y * cellSize;

            var sprite = _sprites.Ghost(GhostColour.Red, Direction.Up, animated);
            _sprites.RenderSprite(g, x, y, sprite);
        }

        public void RenderScore(Graphics g, NPacMan.Game.Game game)
        {
            _scoreBoard.RenderStatic(g);
            _scoreBoard.RenderScores(g, game.Score, 0, 1000);
        }

        public void RenderLives(Graphics g, NPacMan.Game.Game game)
        {
            _scoreBoard.RenderLivesBonus(g, game.Lives);
        }
    }
}
