using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class PacManTests
    {
        private readonly TestGameSettings _gameSettings;
        private readonly TestGameClock _gameClock;

        public PacManTests()
        {
            _gameSettings = new TestGameSettings();
            _gameClock = new TestGameClock();
        }

        [Fact]
        public void PacManStartsInInitialPosition()
        {
            _gameSettings.PacMan = new PacMan((5, 6), Direction.Right);
            var game = new Game(_gameClock, _gameSettings);

            game.PacMan.Should().BeEquivalentTo(new
            {
                Location = new CellLocation(5, 6),
                Direction = Direction.Right
            });
        }


        [Theory]
        [InlineData(Direction.Up, 0, -1)]
        [InlineData(Direction.Down, 0, +1)]
        [InlineData(Direction.Left, -1, 0)]
        [InlineData(Direction.Right, +1, 0)]
        public async Task PacManWalksInFacingDirection(Direction directionToFace, int changeInX, int changeInY)
        {
            var game = new Game(_gameClock, _gameSettings);
            var (x, y) = game.PacMan.Location;

            game.ChangeDirection(directionToFace);

            await _gameClock.Tick();

            game.PacMan.Should().BeEquivalentTo(new
            {
                Location = new CellLocation(x + changeInX, y + changeInY),
                Direction = directionToFace
            });
        }

        [Theory]
        [InlineData(Direction.Up, 0, -1)]
        [InlineData(Direction.Down, 0, +1)]
        [InlineData(Direction.Left, -1, 0)]
        [InlineData(Direction.Right, +1, 0)]
        public async Task PacManCannotMoveIntoWalls(Direction directionToFace, int createWallXOffset, int createWallYOffset)
        {
            var game = new Game(_gameClock, _gameSettings);
            var x = game.PacMan.Location.X;
            var y = game.PacMan.Location.Y;
            var score = game.Score;

            game.ChangeDirection(directionToFace);

            _gameSettings.Walls.Add((x + createWallXOffset, y + createWallYOffset));

            await _gameClock.Tick();

            game.PacMan.Should().BeEquivalentTo(new
            {
                Location = new CellLocation(x, y),
                Direction = directionToFace
            });

            game.Score.Should().Be(score);
        }

        [Fact]
        public async Task PacManIsTeleportedWhenYouWalkIntoAPortal()
        {
            var game = new Game(_gameClock, _gameSettings);
            var x = game.PacMan.Location.X;
            var y = game.PacMan.Location.Y;
            var score = game.Score;

            _gameSettings.Portals.Add((x - 1, y), (15, 15));

            game.ChangeDirection(Direction.Left);

            await _gameClock.Tick();

            game.PacMan.Should().BeEquivalentTo(new
            {
                Location = new CellLocation(14, 15),
                Direction = Direction.Left
            });

            game.Score.Should().Be(score);
        }

        [Theory]
        [InlineData(GameStatus.Dead)]
        [InlineData(GameStatus.Dying)]
        [InlineData(GameStatus.Respawning)]
        public async Task PacManShouldNotMoveInCertainStates(GameStatus state)
        {
            var x = 1;
            var y = 1;

            _gameSettings.InitialGameStatus = state.ToString();
            _gameSettings.PacMan = new PacMan((x, y), Direction.Down);

            var game = new Game(_gameClock, _gameSettings);
            await _gameClock.Tick();

            game.PacMan
                .Should().BeEquivalentTo(new
                {
                    Location = new {
                        X = x,
                        Y = y
                    }
                });
        }

        [Fact]
        public async Task PacManShouldRespawnAfter4Seconds()
        {
            _gameSettings.PacMan = new PacMan((1, 1), Direction.Down);
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", new CellLocation(1, 2), Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);

            await _gameClock.Tick(now.AddSeconds(1));
            await _gameClock.Tick(now.AddSeconds(2));
            await _gameClock.Tick(now.AddSeconds(3));

            if (game.Status != GameStatus.Dying.ToString())
                throw new Exception($"Invalid Game State {game.Status:G}");

            await _gameClock.Tick(now.AddSeconds(4));

            game.Status.Should().Be(GameStatus.Respawning.ToString());
        }

        [Fact]
        public async Task PacManShouldBeAliveAfter4SecondsWhenInRespawning()
        {
            _gameSettings.PacMan = new PacMan((5, 2), Direction.Left);
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", new CellLocation(1, 2), Direction.Right, CellLocation.TopLeft, new GhostGoesRightStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            var now = DateTime.UtcNow;

            await _gameClock.Tick(now);
            await _gameClock.Tick(now);
            await _gameClock.Tick(now.AddSeconds(4));

            if (game.Status != GameStatus.Respawning.ToString())
                throw new Exception($"Invalid Game State {game.Status:G}");

            await _gameClock.Tick(now.AddSeconds(8));

            game.Status.Should().Be(GameStatus.Alive.ToString());
        }

        [Fact]
        public async Task PacManShouldBeBackAtHomeLocationAfter4SecondsWhenBecomingBackAlive()
        {
            _gameSettings.PacMan = new PacMan((5, 2), Direction.Left);
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", new CellLocation(1, 2), Direction.Right, CellLocation.TopLeft, new GhostGoesRightStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);
            await _gameClock.Tick(now);
            await _gameClock.Tick(now.AddSeconds(4));

            if (game.Status != GameStatus.Respawning.ToString())
                throw new Exception($"Invalid Game State {game.Status:G}");

            await _gameClock.Tick(now.AddSeconds(8));

            if (game.Status != GameStatus.Alive.ToString())
                throw new Exception($"Invalid Game State {game.Status:G}");

            game.PacMan.Should().BeEquivalentTo(
                new
                {
                    Location = new {
                        X = 5,
                        Y = 2
                    }
                }
            );
        }
    }
}
