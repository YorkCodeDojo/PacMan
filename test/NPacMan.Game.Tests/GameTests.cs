using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace NPacMan.Game.Tests
{
    public class GameTests
    {
        private readonly TestGameSettings _gameSettings;
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
            _gameSettings = new TestGameSettings();
            _gameClock = new TestGameClock();
            _game = new Game(_gameClock, _gameSettings);
        }

        [Fact]
        public void PacManStartsInInitialPosition()
        {
            _gameSettings.PacMan = new PacMan(5, 6, Direction.Right, PacManStatus.Alive, 3);
            var game = new Game(_gameClock, _gameSettings);

            game.PacMan.Should().BeEquivalentTo(new
            {
                X = 5,
                Y = 6,
                Direction = Direction.Right
            });
        }

        [Fact]
        public void GameStartsWithThreeLives()
        {
            _game.Lives.Should().Be(3);
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

            _gameSettings.Walls.Add((x + createWallXOffset, y + createWallYOffset));

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

            _gameSettings.Coins.Add((x, y + 1));
            _gameClock.Tick();

            _game.Score.Should().Be(10);
        }

        [Fact]
        public void IncrementsScoreBy20WhenTwoCoinsAreCollected()
        {
            var x = _game.PacMan.X;
            var y = _game.PacMan.Y;

            _game.ChangeDirection(Direction.Down);

            _gameSettings.Coins.Add((x, y + 1));
            _gameSettings.Coins.Add((x, y + 2));

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

            _gameSettings.Coins.Add((x, y + 1));

            _gameClock.Tick();

            _game.Coins.Should().NotContain((x, y + 1));
        }

        [Fact]
        public void JustTheCollectedCoinShouldBeCollected()
        {
            var x = _game.PacMan.X;
            var y = _game.PacMan.Y;

            _game.ChangeDirection(Direction.Down);

            _gameSettings.Coins.Add((x, y + 1));
            _gameSettings.Coins.Add((x, y + 2));

            _gameClock.Tick();

            _game.Coins.Should().NotContain((x, y + 1));
            _game.Coins.Should().Contain((x, y + 2));
        }

        [Fact]
        public void GameContainsAllCoins()
        {
            var gameBoard = new TestGameSettings();
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

            _gameSettings.Portals.Add((x - 1, y), (15, 15));

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
            _gameSettings.Width = gameBoardWidth;

            _game.Width.Should().Be(gameBoardWidth);

        }

        [Fact]
        public void TheGameCanReadTheHeightFromTheBoard()
        {
            var gameBoardHeight = 100;
            _gameSettings.Height = gameBoardHeight;

            _game.Height.Should().Be(gameBoardHeight);

        }

        [Fact]
        public void PacManDoesNotLoseALifeWhenNotCollidingWithAGhost()
        {
            var currentLives = _game.Lives;
            var x = _game.PacMan.X + 1;
            var y = _game.PacMan.Y;

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", x, y, CellLocation.TopLeft, new StandingStillGhostStrategy(), new StandingStillGhostStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            game.ChangeDirection(Direction.Left);
            _gameClock.Tick();

            game.Lives.Should().Be(currentLives);
        }

        [Fact]
        public void PacManLosesALifeWhenCollidesWithGhost()
        {
            var currentLives = _game.Lives;
            var x = _game.PacMan.X + 1;
            var y = _game.PacMan.Y;

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", x, y, CellLocation.TopLeft, new StandingStillGhostStrategy(), new StandingStillGhostStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            game.ChangeDirection(Direction.Right);
            _gameClock.Tick();

            game.Lives.Should().Be(currentLives - 1);
            game.PacMan.Status.Should().Be(PacManStatus.Dying);
        }


        [Fact]
        public void PacManLosesALifeWhenCollidesWithGhostWalkingTowardsPacMan()
        {
            var currentLives = _game.Lives;

            // G . . . P
            // . G . P .
            // . . PG . .

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", _gameSettings.PacMan.X - 4, _gameSettings.PacMan.Y, CellLocation.TopLeft, new GhostGoesRightStrategy(), new StandingStillGhostStrategy()));

            var game = new Game(_gameClock, _gameSettings);

            game.ChangeDirection(Direction.Left);
            _gameClock.Tick();
            _gameClock.Tick();

            game.Lives.Should().Be(currentLives - 1);
        }

        [Fact]
        public void PacManLosesALifeWhenCollidesWithGhostWhenPacManIsFacingAWall()
        {
            var currentLives = _game.Lives;

            var x = _game.PacMan.X;
            var y = _game.PacMan.Y;

            _gameSettings.Walls.Add((x, y - 1));
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", x - 1, y, CellLocation.TopLeft, new GhostGoesRightStrategy(), new StandingStillGhostStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            game.ChangeDirection(Direction.Up);
            _gameClock.Tick();

            game.Lives.Should().Be(currentLives - 1);
        }


        [Fact]
        public void PacManDoesNotCollectCoinAndScoreStaysTheSameWhenCollidesWithGhost()
        {
            var score = _game.Score;
            var x = _game.PacMan.X + 1;
            var y = _game.PacMan.Y;

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", x, y, CellLocation.TopLeft, new StandingStillGhostStrategy(), new StandingStillGhostStrategy()));
            _gameSettings.Coins.Add((x, y));

            var game = new Game(_gameClock, _gameSettings);
            game.ChangeDirection(Direction.Right);
            _gameClock.Tick();

            using var _ = new AssertionScope();
            game.Coins.Should().Contain((x, y));
            game.Score.Should().Be(score);

        }

        [Fact]
        public void GhostMovesInDirectionOfStrategy()
        {
            var strategy = new GhostGoesRightStrategy();
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", 0, 0, CellLocation.TopLeft, strategy, new StandingStillGhostStrategy()));

            var game = new Game(_gameClock, _gameSettings);

            _gameClock.Tick();
            game.Ghosts["Ghost1"].Should().BeEquivalentTo(new
            {
                X = 1,
                Y = 0,
            });
        }


        [Fact]
        public void GhostShouldNotMoveWhenPacManIsDying()
        {
            var x = 1;
            var y = 1;

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", x, y, CellLocation.TopLeft, new DirectChaseToPacManStrategy(), new StandingStillGhostStrategy()));
            _gameSettings.PacMan = new PacMan(3,3,Direction.Down, PacManStatus.Dying, 1);

            var game = new Game(_gameClock, _gameSettings);
            _gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts.Values.First()
                .Should().BeEquivalentTo(new
                {
                    X = x,
                    Y = y
                });
        }

        [Theory]
        [InlineData(PacManStatus.Dead)]
        [InlineData(PacManStatus.Dying)]
        [InlineData(PacManStatus.Respawning)]
        public void PacManShouldNotMoveInCertainStates(PacManStatus state)
        {
            var x = 1;
            var y = 1;

            _gameSettings.PacMan = new PacMan(x, y, Direction.Down, state, 1);

            var game = new Game(_gameClock, _gameSettings);
            _gameClock.Tick();

            game.PacMan
                .Should().BeEquivalentTo(new
                {
                    X = x,
                    Y = y
                });
        }

        [Fact]
        public void PacManShouldNotLoseLifesWhenAlreadyIsDying()
        {
            var expectedLife = 1;
            var x = 1;
            var y = 1;
        
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", x, y, CellLocation.TopLeft, new NPacMan.Game.StandingStillGhostStrategy(), new StandingStillGhostStrategy()));
            _gameSettings.PacMan = new PacMan(1, 1, Direction.Down, PacManStatus.Dying, expectedLife);
        
            var game = new Game(_gameClock, _gameSettings);
            _gameClock.Tick();
        
            using var _ = new AssertionScope();
            game.Lives.Should().Be(expectedLife);
        }

        [Fact]
        public void PacManShouldRespawnAfter4Seconds()
        {
            _gameSettings.PacMan = new PacMan(1, 1, Direction.Down, PacManStatus.Alive, 1);
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", 1, 2, CellLocation.TopLeft, new NPacMan.Game.StandingStillGhostStrategy(), new StandingStillGhostStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            var now = DateTime.UtcNow;
            _gameClock.Tick(now);

            _gameClock.Tick(now.AddSeconds(1));
            _gameClock.Tick(now.AddSeconds(2));
            _gameClock.Tick(now.AddSeconds(3));

            if (game.PacMan.Status != PacManStatus.Dying)
                throw new Exception($"Invalid PacMan State {game.PacMan.Status:G}");

            _gameClock.Tick(now.AddSeconds(4));

            game.PacMan.Status.Should().Be(PacManStatus.Respawning);
        }


        [Fact]
        public void GhostShouldBeAbleToMoveWhenPacManIsReSpawning()
        {
            _gameSettings.PacMan = new PacMan(1, 1, Direction.Left, PacManStatus.Respawning, 1);
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", 1, 2, CellLocation.TopLeft, new GhostGoesRightStrategy(), new StandingStillGhostStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            _gameClock.Tick();
            
            game.Ghosts.Values.First()
                .Should().BeEquivalentTo(new
                {
                    X = 2,
                    Y = 2
                });
        }

        [Fact]
        public void GhostShouldGoHome()
        {
            _gameSettings.PacMan = new PacMan(1, 1, Direction.Down, PacManStatus.Alive, 2);
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", 1, 2, new CellLocation(4, 4), new NPacMan.Game.StandingStillGhostStrategy(), new MoveHomeGhostStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            var now = DateTime.UtcNow;
            _gameClock.Tick(now);
            _gameClock.Tick(now.AddSeconds(4));

            if (game.PacMan.Status != PacManStatus.Respawning)
                throw new Exception($"Invalid PacMan State {game.PacMan.Status:G}");

            _gameClock.Tick();
            _gameClock.Tick();
            _gameClock.Tick();
            _gameClock.Tick();
            _gameClock.Tick();

            game.Ghosts.Values.First()
                .Should().BeEquivalentTo(new
                {
                    X = 4,
                    Y = 4
                });
        }

    }

}
