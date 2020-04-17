using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NPacMan.Game.Tests
{
    public class TestGameBoard : IGameBoard
    {
        public List<(int x, int y)> Walls { get; set; }
            = new List<(int x, int y)>();

        IReadOnlyCollection<(int x, int y)> IGameBoard.Walls
            => this.Walls;

        public List<(int x, int y)> Coins { get; set; }
            = new List<(int x, int y)>();

        IReadOnlyCollection<(int x, int y)> IGameBoard.Coins
            => this.Coins;

        public Dictionary<(int x, int y), (int x, int y)> Portals { get; set; }
            = new Dictionary<(int x, int y), (int x, int y)>();

        IReadOnlyDictionary<(int x, int y), (int x, int y)> IGameBoard.Portals
            => this.Portals;
    }

    public class TestGameClock : IGameClock
    {
        private List<Action> _actions = new List<Action>();

        public void Subscribe(Action action)
        {
            _actions.Add(action);
        }

        public void Tick()
        {
            _actions.ForEach(x => x());
        }
    }
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
                X = 15,
                Y = 15,
                Direction = Direction.Left
            });

            _game.Score.Should().Be(score);
        }
    }
}
