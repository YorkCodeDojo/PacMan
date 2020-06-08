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
            _gameSettings.PacMan = new PacMan((5, 6), Direction.Right);
            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
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
            game.StartGame();

            var (x, y) = game.PacMan.Location;

            await game.ChangeDirection(directionToFace);

            await _gameClock.Tick();

            game.PacMan.Should().BeEquivalentTo(new
            {
                Location = new CellLocation(x + changeInX, y + changeInY),
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
            var location = _gameSettings.PacMan.Location;
            _gameSettings.PacMan = new PacMan(location, directionToFace);
            _gameSettings.Walls.Add(location + directionToFace);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await _gameClock.Tick();

            game.PacMan.Should().BeEquivalentTo(new
            {
                Location = location,
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
            var location = _gameSettings.PacMan.Location;
            _gameSettings.PacMan = new PacMan(location, directionToFace);
            _gameSettings.Doors.Add(location + directionToFace);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await _gameClock.Tick();

            game.PacMan.Should().BeEquivalentTo(new
            {
                Location = location,
                Direction = directionToFace
            });
        }

        [Fact]
        public async Task PacManIsTeleportedWhenYouWalkIntoAPortal()
        {
            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
            var x = game.PacMan.Location.X;
            var y = game.PacMan.Location.Y;
            var score = game.Score;

            _gameSettings.Portals.Add((x - 1, y), (15, 15));

            await game.ChangeDirection(Direction.Left);

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

            _gameSettings.InitialGameStatus = state;
            _gameSettings.PacMan = new PacMan((x, y), Direction.Down);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
            await _gameClock.Tick();

            game.PacMan
                .Should().BeEquivalentTo(new
                {
                    Location = new
                    {
                        X = x,
                        Y = y
                    }
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

        [Fact]
        public async Task PacManShouldBeDeadWhenNoLivesLeft()
        {
            _gameSettings.InitialLives = 1;
            _gameSettings.PacMan = new PacMan((5, 2), Direction.Left);
            _gameSettings.Ghosts.Add(GhostBuilder.New().WithLocation((1, 2)).WithChaseStrategyRight().Create());

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
            var now = DateTime.UtcNow;

            await _gameClock.Tick(now);
            await _gameClock.Tick(now);
            await _gameClock.Tick(now.AddSeconds(4));

            game.Status.Should().Be(GameStatus.Dead);
        }

        [Theory]
        [InlineData(1, 200)]
        [InlineData(2, 600)]
        [InlineData(3, 1400)]
        [InlineData(4, 3000)]
        [InlineData(5, 6200)]
        public async Task ScoreShouldIncreaseExponentiallyAfterEatingEachGhost(int numberOfGhosts, int totalScore)
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

            game.Score.Should().Be(scoreBeforeGhost + totalScore);
        }
    }
}




