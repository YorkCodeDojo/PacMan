using System;
using System.Drawing;
using NPacMan.Game;
using NPacMan.SharedUi.Map;

namespace NPacMan.UI
{
    public class BoardRenderer
    {
        private readonly Font _scoreFont = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

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
                    _sprites.RenderSprite(g, posX, posY, _sprites.Map(_mapLayout.BoardPieceToDisplay(x, y)));
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
                var x = coin.X * cellSize;
                var y = coin.Y * cellSize;

                _sprites.RenderSprite(g, x,y, _sprites.Coin());
            }
        }

        public void RenderPowerPills(Graphics g, NPacMan.Game.Game game)
        {
            var cellSize = Sprites.PixelGrid;

            var powerPills = game.PowerPills;

            foreach (var powerPill in powerPills)
            {
                var x = powerPill.X * cellSize;
                var y = powerPill.Y * cellSize;

                _sprites.RenderSprite(g, x,y, _sprites.PowerPill());
            }
        }

        private int _pacManAnimation = 0;
        private int _pacManAnimationDelay = 0;
        public void RenderPacMan(Graphics g, NPacMan.Game.Game game)
        {
            var cellSize = Sprites.PixelGrid;

            var x = game.PacMan.Location.X * cellSize;
            var y = game.PacMan.Location.Y * cellSize;

            _pacManAnimationDelay++;
            if (_pacManAnimationDelay == 3)
            {
                if (game.Status == GameStatus.Alive)
                {
                    _pacManAnimation = (_pacManAnimation + 1) % 4;
                }
                else
                {
                    _pacManAnimation = (_pacManAnimation + 1) % 4;
                }
                _pacManAnimationDelay = 0;
            }

            _sprites.RenderSprite(g, x, y, _sprites.PacMan(game.PacMan.Direction, _pacManAnimation, game.Status == GameStatus.Dying));
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
            var x = ghost.Location.X * cellSize;
            var y = ghost.Location.Y * cellSize;

            var ghostColour = (ghost.Name, ghost.Edible) switch
            {
                (GhostNames.Blinky, false) => GhostColour.Red,
                (GhostNames.Inky, false) => GhostColour.Cyan,
                (GhostNames.Pinky, false) => GhostColour.Pink,
                (GhostNames.Clyde, false) => GhostColour.Orange,
                (_, true) => GhostColour.BlueFlash,
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
