using System;
using System.Threading.Tasks;
using FluentAssertions;
using NPacMan.Game.Tests.Helpers;
using Xunit;
using static NPacMan.Game.Tests.Helpers.Ensure;

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
            var initialPosition = _gameSettings.PacMan.Location;
            
            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            gameHarness.Game.PacMan.Should().BeEquivalentTo(new
            {
                Location = initialPosition,
                Direction = Direction.Right
            });
        }

        [Theory]
        [InlineData(Direction.Up)]
        [InlineData(Direction.Down)]
        [InlineData(Direction.Left)]
        [InlineData(Direction.Right)]
        public async Task PacManWalksInFacingDirection(Direction directionToFace)
        {
            var initialPosition = _gameSettings.PacMan.Location;

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(directionToFace);

            await gameHarness.Move();

            gameHarness.Game.PacMan.Should().BeEquivalentTo(new
            {
                Location = initialPosition + directionToFace,
                Direction = directionToFace
            });
        }

        [Theory]
        [InlineData(Direction.Up)]
        [InlineData(Direction.Down)]
        [InlineData(Direction.Left)]
        [InlineData(Direction.Right)]
        public async Task PacManCannotMoveIntoWalls(Direction directionToFace)
        {
            var initialPosition = _gameSettings.PacMan.Location;
            _gameSettings.PacMan = new PacMan(initialPosition, directionToFace);
            _gameSettings.Walls.Add(initialPosition + directionToFace);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.NOP();

            gameHarness.Game.PacMan.Should().BeEquivalentTo(new
            {
                Location = initialPosition,
                Direction = directionToFace
            });
        }

        [Theory]
        [InlineData(Direction.Up)]
        [InlineData(Direction.Down)]
        [InlineData(Direction.Left)]
        [InlineData(Direction.Right)]
        public async Task PacManCannotMoveIntoDoors(Direction directionToFace)
        {
            var initialPosition = _gameSettings.PacMan.Location;
            _gameSettings.PacMan = new PacMan(initialPosition, directionToFace);
            _gameSettings.Doors.Add(initialPosition + directionToFace);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.NOP();

            gameHarness.Game.PacMan.Should().BeEquivalentTo(new
            {
                Location = initialPosition,
                Direction = directionToFace
            });
        }

        [Fact]
        public async Task PacManIsTeleportedWhenYouWalkIntoAPortal()
        {
            var initialPosition = _gameSettings.PacMan.Location;

            var farEndOfPortal = new CellLocation(14, 15);
            var portalExit = farEndOfPortal.Left;

            _gameSettings.Portals.Add(initialPosition.Left, farEndOfPortal);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.Move();

            gameHarness.Game.PacMan.Should().BeEquivalentTo(new
            {
                Location = portalExit,
                Direction = Direction.Left
            });
        }

        [Theory]
        [InlineData(GameStatus.Dead)]
        [InlineData(GameStatus.Dying)]
        [InlineData(GameStatus.Respawning)]   //TODO: Are there more states we could test for here?
        public async Task PacManShouldNotMoveInCertainStates(GameStatus state)
        {
            var initialPosition = _gameSettings.PacMan.Location;

            _gameSettings.InitialGameStatus = state;

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.NOP();

            gameHarness.Game.PacMan
                .Should().BeEquivalentTo(new
                {
                    Location = initialPosition
                });
        }

        [Fact]
        public async Task PacManShouldRespawnAfter4Seconds()
        {
            _gameSettings.PacMan = new PacMan((1, 1), Direction.Down);
            _gameSettings.Ghosts.Add(GhostBuilder.New().WithLocation((1,2)).Create());

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);

            await _gameClock.Tick(now.AddSeconds(1));
            await _gameClock.Tick(now.AddSeconds(2));
            await _gameClock.Tick(now.AddSeconds(3));

            if (game.Status != GameStatus.Dying)
                throw new Exception($"Invalid Game State {game.Status:G}");

            await _gameClock.Tick(now.AddSeconds(4));

            game.Status.Should().Be(GameStatus.Respawning);
        }

        [Fact]
        public async Task WhenPacManDiesTheGameNotificationShouldFire()
        {
            _gameSettings.PacMan = new PacMan((1, 1), Direction.Down);
            _gameSettings.Ghosts.Add(GhostBuilder.New().WithLocation((1, 2)).Create());

            var numberOfNotificationsTriggered = 0;

            var game = new Game(_gameClock, _gameSettings);
            game.Subscribe(GameNotification.Dying, () => numberOfNotificationsTriggered++);
            game.StartGame();
            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);

            await _gameClock.Tick(now.AddSeconds(1));
            await _gameClock.Tick(now.AddSeconds(2));
            await _gameClock.Tick(now.AddSeconds(3));

            if (game.Status != GameStatus.Dying)
                throw new Exception($"Invalid Game State {game.Status:G} should be {nameof(GameStatus.Dying)}");

            numberOfNotificationsTriggered.Should().Be(1);
        }

        [Fact]
        public async Task WhenPacManRespawnsTheGameNotificationShouldFire()
        {
            _gameSettings.PacMan = new PacMan((1, 1), Direction.Down);
            _gameSettings.Ghosts.Add(GhostBuilder.New().WithLocation((1, 2)).Create());

            var numberOfNotificationsTriggered = 0;

            var game = new Game(_gameClock, _gameSettings);
            game.Subscribe(GameNotification.Respawning, () => numberOfNotificationsTriggered++);
            game.StartGame();
            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);

            await _gameClock.Tick(now.AddSeconds(1));
            await _gameClock.Tick(now.AddSeconds(2));
            await _gameClock.Tick(now.AddSeconds(3));

            if (game.Status != GameStatus.Dying)
                throw new Exception($"Invalid Game State {game.Status:G} should be {nameof(GameStatus.Dying)}");

            await _gameClock.Tick(now.AddSeconds(4));

            if (game.Status != GameStatus.Respawning)
                throw new Exception($"Invalid Game State {game.Status:G} should be {nameof(GameStatus.Respawning)}");

            numberOfNotificationsTriggered.Should().Be(1);
        }

        [Fact]
        public async Task PacManShouldBeAliveAfter4SecondsWhenInRespawning()
        {
            _gameSettings.PacMan = new PacMan((5, 2), Direction.Left);
            _gameSettings.Ghosts.Add(GhostBuilder.New().WithLocation((1, 2)).WithChaseStrategyRight().Create());

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
            var now = DateTime.UtcNow;

            await _gameClock.Tick(now);
            await _gameClock.Tick(now);
            await _gameClock.Tick(now.AddSeconds(4));

            if (game.Status != GameStatus.Respawning)
                throw new Exception($"Invalid Game State {game.Status:G}");

            await _gameClock.Tick(now.AddSeconds(8));

            game.Status.Should().Be(GameStatus.Alive);
        }

        [Fact]
        public async Task PacManShouldBeBackAtHomeLocationAfter4SecondsWhenBecomingBackAlive()
        {
            _gameSettings.PacMan = new PacMan((5, 2), Direction.Left);
            _gameSettings.Ghosts.Add(GhostBuilder.New().WithLocation((1, 2)).WithChaseStrategyRight().Create());

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);
            await _gameClock.Tick(now);
            await _gameClock.Tick(now.AddSeconds(4));

            if (game.Status != GameStatus.Respawning)
                throw new Exception($"Invalid Game State {game.Status:G}");

            await _gameClock.Tick(now.AddSeconds(8));

            if (game.Status != GameStatus.Alive)
                throw new Exception($"Invalid Game State {game.Status:G}");

            game.PacMan.Should().BeEquivalentTo(
                new
                {
                    Location = new
                    {
                        X = 5,
                        Y = 2
                    }
                }
            );
        }

        [Theory]
        [InlineData(Direction.Up)]
        [InlineData(Direction.Down)]
        [InlineData(Direction.Left)]
        [InlineData(Direction.Right)]
        public async Task PacManCantTurnToFaceWall(Direction direction)
        {
            var expectedDirection = direction.Opposite();
            var pacManLocation = new CellLocation(1, 1);
            _gameSettings.PacMan = new PacMan(pacManLocation, expectedDirection);
            _gameSettings.Walls.Add(pacManLocation + direction);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(direction);

            game.PacMan.Should().BeEquivalentTo(new
            {
                Location = pacManLocation,
                Direction = expectedDirection
            });
        }

        [Theory]
        [InlineData(Direction.Up)]
        [InlineData(Direction.Down)]
        [InlineData(Direction.Left)]
        [InlineData(Direction.Right)]
        public async Task PacManCantTurnToFaceDoor(Direction direction)
        {
            var expectedDirection = direction.Opposite();
            var pacManLocation = new CellLocation(1, 1);
            _gameSettings.PacMan = new PacMan(pacManLocation, expectedDirection);
            _gameSettings.Doors.Add(pacManLocation + direction);
            var game = new Game(_gameClock, _gameSettings);

            game.StartGame();

            await game.ChangeDirection(direction);

            game.PacMan.Should().BeEquivalentTo(new
            {
                Location = pacManLocation,
                Direction = expectedDirection
            });
        }

        [Theory]
        [InlineData(1, 200, 200)]
        [InlineData(2, 400, 600)]
        [InlineData(3, 800, 1400)]
        [InlineData(4, 1600, 3000)]
        [InlineData(5, 3200, 6200)]
        public async Task ScoreShouldIncreaseExponentiallyAfterEatingEachGhost(int numberOfGhosts, int ghostScore, int totalScore)
        {
            var ghostStart = _gameSettings.PacMan.Location.Left.Left.Left;
            var ghosts = GhostBuilder.New().WithLocation(ghostStart).WithChaseStrategyRight()
                .CreateMany(numberOfGhosts);
            
            _gameSettings.Ghosts.AddRange(ghosts);
            
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.FarAway());
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Left);

            await _gameClock.Tick();

            var scoreBeforeGhost = game.Score;

            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left);
            for (int g = 0; g < numberOfGhosts; g++)
            {
                WeExpectThat(game.Ghosts.Values).IsAt(ghostStart.Right);
            }

            await _gameClock.Tick();

            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left.Left);

            game.Should().BeEquivalentTo(new {
                Score = scoreBeforeGhost + totalScore,
                PointsForEatingLastGhost = ghostScore
            });
            
        }
    }
}




