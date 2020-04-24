using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace NPacMan.Game.Tests
{
    public class GameTests
    {
        private readonly TestGameBoard _gameBoard;
        private readonly TestGameClock _gameClock;
        private readonly Game _game;

        // 1. Walks in facing direction
        // 2. Does not walk when wall in the way
        // 3. Increments score by 10 when a coin when collected.
        // 4. Coin is removed from game when collected.
        // 5. Game ends when all coins are collected.
        // 6. Can teleport from left to right

        public GameTests()
        {
            _gameBoard = new TestGameBoard();
            _gameClock = new TestGameClock();
            _game = new Game(_gameClock, _gameBoard);
        }

        [Theory]
        [InlineData(Direction.Up, 0, -1)]
        [InlineData(Direction.Down, 0, +1)]
        [InlineData(Direction.Left, -1, 0)]
        [InlineData(Direction.Right, +1, 0)]
        public void WalksInFacingDirection(Direction directionToFace, int changeInX, int changeInY)
        {
            var x = _game.PacMan.X;
            var y = _game.PacMan.Y;

            _game.ChangeDirection(directionToFace);

            _gameClock.Tick();

            _game.PacMan.Should().BeEquivalentTo(new
            {
                X = x + changeInX,
                Y = y + changeInY,
                Direction = directionToFace
            });
        }

        [Theory]
        [InlineData(Direction.Up, 0, -1)]
        [InlineData(Direction.Down, 0, +1)]
        [InlineData(Direction.Left, -1, 0)]
        [InlineData(Direction.Right, +1, 0)]
        public void CannotMoveIntoWalls(Direction directionToFace, int createWallXOffset, int createWallYOffset)
        {
            var x = _game.PacMan.X;
            var y = _game.PacMan.Y;
            var score = _game.Score;

            _game.ChangeDirection(directionToFace);

            _gameBoard.Walls.Add((x + createWallXOffset, y + createWallYOffset));

            _gameClock.Tick();

            _game.PacMan.Should().BeEquivalentTo(new
            {
                X = x,
                Y = y,
                Direction = directionToFace
            });

            _game.Score.Should().Be(score);
        }

        [Fact]
        public void ScoreDoesNotChangeWhenNoCoinIsCollected()
        {
            var x = _game.PacMan.X;
            var y = _game.PacMan.Y;
            var score = _game.Score;

            _game.ChangeDirection(Direction.Down);

            _gameClock.Tick();

            _game.Score.Should().Be(score);
        }

        [Fact]
        public void IncrementsScoreBy10WhenCoinCollected()
        {
            var x = _game.PacMan.X;
            var y = _game.PacMan.Y;

            _game.ChangeDirection(Direction.Down);

            _gameBoard.Coins.Add((x, y + 1));
            _gameClock.Tick();

            _game.Score.Should().Be(10);
        }

        [Fact]
        public void IncrementsScoreBy20WhenTwoCoinsAreCollected()
        {
            var x = _game.PacMan.X;
            var y = _game.PacMan.Y;

            _game.ChangeDirection(Direction.Down);

            _gameBoard.Coins.Add((x, y + 1));
            _gameBoard.Coins.Add((x, y + 2));

            _gameClock.Tick();
            _gameClock.Tick();

            _game.Score.Should().Be(20);
        }
        
        [Fact]
        public void CoinShouldBeCollected()
        {
            var x = _game.PacMan.X;
            var y = _game.PacMan.Y;

            _game.ChangeDirection(Direction.Down);

            _gameBoard.Coins.Add((x, y + 1));

            _gameClock.Tick();

            _game.Coins.Should().NotContain((x, y + 1));
        }

        [Fact]
        public void JustTheCollectedCoinShouldBeCollected()
        {
            var x = _game.PacMan.X;
            var y = _game.PacMan.Y;

            _game.ChangeDirection(Direction.Down);

            _gameBoard.Coins.Add((x, y + 1));
            _gameBoard.Coins.Add((x, y + 2));

            _gameClock.Tick();

            _game.Coins.Should().NotContain((x, y + 1));
            _game.Coins.Should().Contain((x, y + 2));
        }

        [Fact]
        public void GameContainsAllCoins()
        {
            var gameBoard = new TestGameBoard();
            gameBoard.Coins.Add((1, 1));
            gameBoard.Coins.Add((1, 2));
            gameBoard.Coins.Add((2, 2));

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, gameBoard);

            gameClock.Tick();

            game.Coins.Should().BeEquivalentTo(
                (1, 1),
                (1, 2),
                (2, 2)
            );
        }

        [Fact]
        public void TeleportWhenYouWalkIntoAPortal()
        {
            var x = _game.PacMan.X;
            var y = _game.PacMan.Y;
            var score = _game.Score;

            _gameBoard.Portals.Add((x - 1, y), (15, 15));

            _game.ChangeDirection(Direction.Left);

            _gameClock.Tick();

            _game.PacMan.Should().BeEquivalentTo(new
            {
                X = 14,
                Y = 15,
                Direction = Direction.Left
            });

            _game.Score.Should().Be(score);
        }


        [Fact]
        public void TheGameCanReadTheWidthFromTheBoard()
        {
            var gameBoardWidth = 100;
            _gameBoard.Width = gameBoardWidth;

            _game.Width.Should().Be(gameBoardWidth);

        }

        [Fact]
        public void TheGameCanReadTheHeightFromTheBoard()
        {
            var gameBoardHeight = 100;
            _gameBoard.Height = gameBoardHeight;

            _game.Height.Should().Be(gameBoardHeight);

        }

        [Fact]
        public void PacManDoesNotLoseALifeWhenNotCollidingWithAGhost()
        {
            var currentLives = _game.Lives;
            var x = _game.PacMan.X + 1;
            var y = _game.PacMan.Y;

            _gameBoard.Ghosts.Add(new Ghost(x, y));

            _game.ChangeDirection(Direction.Left);
            _gameClock.Tick();

            _game.Lives.Should().Be(currentLives);
        }

        [Fact]
        public void PacManLosesALifeWhenCollidesWithGhost()
        {
            var currentLives = _game.Lives;
            var x = _game.PacMan.X + 1;
            var y = _game.PacMan.Y;

            _gameBoard.Ghosts.Add(new Ghost(x, y));

            _game.ChangeDirection(Direction.Right);
            _gameClock.Tick();

            _game.Lives.Should().Be(currentLives - 1);
        }


        [Fact]
        public void PacManDoesNotCollectCoinAndScoreStaysTheSameWhenCollidesWithGhost()
        {
            var score = _game.Score;
            var x = _game.PacMan.X + 1;
            var y = _game.PacMan.Y;

            _gameBoard.Ghosts.Add(new Ghost(x, y));
            _gameBoard.Coins.Add((x, y));

            _game.ChangeDirection(Direction.Right);
            _gameClock.Tick();

            using var _ = new AssertionScope();
            _game.Coins.Should().Contain((x, y));
            _game.Score.Should().Be(score);
        }
    }
}
