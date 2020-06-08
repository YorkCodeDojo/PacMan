using System.Collections.Generic;
using NPacMan.Game;
using NPacMan.SharedUi.Map;

namespace NPacMan.SharedUi
{
    public class BoardRenderer
    {
        private bool animated = false;

        private Sprites _sprites;
        private ScoreBoard _scoreBoard;
        private MapLayout _mapLayout;

        private Display _display;

        public int DisplayHeight => _display.Height;
        public int DisplayWidth => _display.Width;

        public IEnumerable<SpriteDisplay> BackgroundToUpdate => _display.GetBackgroundToDraw();
        public IEnumerable<SpriteDisplay> SpritesToDisplay => _display.SpritesToDisplay;

        private int _boardY = 3;

        private int _ticks = 0;

        public BoardRenderer()
        {
            _display = new Display();

            _sprites = new Sprites();
            _scoreBoard = new ScoreBoard(_display, _sprites);
            _mapLayout = new MapLayout();
        }
        
        public void RenderStart(Game.Game game)
        {
            // Resize if necessary (required for first tick)
            _display.Size(game.Width, game.Height+5);

            // Blank the rows outside of the board area
            foreach(var row in new int[] { 0,1,2, game.Height+3, game.Height+4})
            ClearRow(row);

            // Build the static background
            RenderScore(game);

            _ticks++;

            if(game.Status == GameStatus.ChangingLevel && _ticks % 10 < 5)
            {
                FlashBoard();
            }
            else
            {
                RenderWalls(game);
                RenderCoins(game);
                RenderPowerPills(game);
                RenderGhosts(game);
            }

            RenderFruit(game);

            // Add the sprites
            RenderPacMan(game);
            RenderLives(game);
        }

        private void ClearRow(int row)
        {
            for (int x = 0; x < _mapLayout.DisplayWidth; x++)
            {
                _display.DrawOnBackground(x,row, _sprites.Map(BoardPiece.Blank));
            }
        }

        private void FlashBoard()
        {
            for (int y = 0; y < _mapLayout.DisplayHeight; y++)
            {
                for (int x = 0; x < _mapLayout.DisplayWidth; x++)
                {
                    _display.DrawOnBackground(x, y+_boardY, _sprites.Map(BoardPiece.Blank));
                }
            }
        }

        private void RenderWalls(Game.Game game)
        {
            _mapLayout.UpdateFromGame(game);

            for (int y = 0; y < _mapLayout.DisplayHeight; y++)
            {
                for (int x = 0; x < _mapLayout.DisplayWidth; x++)
                {
                    _display.DrawOnBackground(x, y+_boardY, _sprites.Map(_mapLayout.BoardPieceToDisplay(x, y)));
                }
            }
        }

        private void RenderCoins(Game.Game game)
        {
            var coins = game.Coins;

            foreach (var coin in coins)
            {
                _display.DrawOnBackground(coin.X,coin.Y+_boardY, _sprites.Coin());
            }
        }

        private void RenderFruit(Game.Game game)
        {
            var fruits = game.Fruits;

            foreach (var fruit in fruits)
            {
                _display.AddSprite(fruit.Location.X, fruit.Location.Y+_boardY, _sprites.Bonus[(int)fruit.Type]);
            }
        }

        private void RenderPowerPills(Game.Game game)
        {
            var powerPills = game.PowerPills;

            foreach (var powerPill in powerPills)
            {
                _display.DrawOnBackground(powerPill.X, powerPill.Y+_boardY, _sprites.PowerPill());
            }
        }
        
        private int _pacManAnimation = 0;
        private int _pacManAnimationDelay = 0;

        private void RenderPacMan(Game.Game game)
        {
            var x = game.PacMan.Location.X;
            var y = game.PacMan.Location.Y;

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

            _display.AddSprite(x, y+_boardY, _sprites.PacMan(game.PacMan.Direction, _pacManAnimation, game.Status == GameStatus.Dying));
        }

        private void RenderGhosts(Game.Game game)
        {
            animated = !animated;

            foreach (var ghost in game.Ghosts.Values)
            {
                RenderGhost(ghost);
            }
        }

        private void RenderGhost(Ghost ghost)
        {
            var ghostColour = (ghost.Name, ghost.Edible) switch
            {
                (GhostNames.Blinky, false) => GhostColour.Red,
                (GhostNames.Inky, false) => GhostColour.Cyan,
                (GhostNames.Pinky, false) => GhostColour.Pink,
                (GhostNames.Clyde, false) => GhostColour.Orange,
                (_, true) => GhostColour.BlueFlash,
                _ => GhostColour.Red,
            };

            var sprite = _sprites.Ghost(ghostColour, ghost.Direction, animated);
            _display.AddSprite(ghost.Location.X, ghost.Location.Y+_boardY, sprite);
        }

        private void RenderScore(Game.Game game)
        {
            _scoreBoard.RenderStatic();
            _scoreBoard.RenderScores(game.Score, 0, 1000);
        }

        private void RenderLives(Game.Game game)
        {
            // The large sprite in the score board span two grid squares
            // rather than being central to one. Adding 0.5 grid square to compensate

            _scoreBoard.RenderLivesBonus(game.Lives, game.Height + _boardY + 0.5m);
        }
    }
}
