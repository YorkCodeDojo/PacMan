using System;
using System.Drawing;
using NPacMan.Game;
using NPacMan.UI.Map;

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
        private MapLayout _mapLayout;

        public BoardRenderer()
        {
            _sprites = new Sprites();
            _scoreBoard = new ScoreBoard(_sprites);
            _mapLayout = new MapLayout();
        }

        public void RenderWalls(Graphics g, NPacMan.Game.Game game)
        {
            _mapLayout.UpdateFromGame(game);
            var cellSize = Sprites.PixelGrid;

            for (int y = 0; y < _mapLayout.DisplayHeight; y++)
            {
                for (int x = 0; x < _mapLayout.DisplayWidth; x++)
                {
                    var posX = x * cellSize;
                    var posY = y * cellSize;
                    _sprites.RenderSprite(g, posX, posY, _sprites.Map(_mapLayout.BoardPieceToDisplay(x,y)));
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

            var ghostColour = ghost.Name switch
            {
                "Blinky" => GhostColour.Red,
                "Inky" => GhostColour.Cyan,
                "Pinky" => GhostColour.Pink,
                "Clyde" => GhostColour.Orange,
                _ => GhostColour.Red,
            };

            var sprite = _sprites.Ghost(ghostColour, Direction.Up, animated);
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
