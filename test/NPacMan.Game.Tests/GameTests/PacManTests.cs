using System;
using System.Threading.Tasks;
using FluentAssertions;
using NPacMan.Game.Tests.Helpers;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class PacManTests
    {
        private readonly TestGameSettings _gameSettings;

        public PacManTests()
        {
            _gameSettings = new TestGameSettings();
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
            var initialPosition = _gameSettings.PacMan.Location;

            var ghost = GhostBuilder.New().WithLocation(initialPosition.Below).Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Down);

            await gameHarness.GetEatenByGhost(ghost);

            await gameHarness.WaitFor(TimeSpan.FromSeconds(3));

            gameHarness.EnsureGameStatus(GameStatus.Dying);

            await gameHarness.WaitFor(TimeSpan.FromSeconds(1));

            gameHarness.Game.Status.Should().Be(GameStatus.Respawning);
        }

        [Fact]
        public async Task WhenPacManDiesTheGameNotificationShouldFire()
        {
            var initialPosition = _gameSettings.PacMan.Location;

            var ghost = GhostBuilder.New().WithLocation(initialPosition.Below).Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Down);

            await gameHarness.AssertSingleNotificationFires(GameNotification.Dying, async () =>
            {
                await gameHarness.GetEatenByGhost(ghost);
            });
        }

        [Fact]
        public async Task WhenPacManRespawnsTheGameNotificationShouldFire()
        {
            var initialPosition = _gameSettings.PacMan.Location;

            var ghost = GhostBuilder.New().WithLocation(initialPosition.Below).Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Down);
            await gameHarness.GetEatenByGhost(ghost);

            await gameHarness.AssertSingleNotificationFires(GameNotification.Respawning, async () =>
            {
                await gameHarness.WaitToFinishDying();
            });
        }

        [Fact]
        public async Task PacManShouldBeAliveAfter4SecondsWhenInRespawning()
        {
            var initialPosition = _gameSettings.PacMan.Location;

            var ghost = GhostBuilder.New()
                                    .WithLocation(initialPosition.Left.Left.Left.Left)
                                    .WithChaseStrategyRight()
                                    .Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.Move();

            await gameHarness.GetEatenByGhost(ghost);

            await gameHarness.WaitToFinishDying();

            gameHarness.EnsureGameStatus(GameStatus.Respawning);

            await gameHarness.WaitToRespawn();

            gameHarness.Game.Status.Should().Be(GameStatus.Alive);
        }

        [Fact]
        public async Task PacManShouldBeBackAtHomeLocationAfter4SecondsWhenBecomingBackAlive()
        {
            var initialPosition = _gameSettings.PacMan.Location;

            var ghost = GhostBuilder.New()
                                    .WithLocation(initialPosition.Left.Left.Left.Left)
                                    .WithChaseStrategyRight()
                                    .Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.Move();

            await gameHarness.GetEatenByGhost(ghost);

            await gameHarness.WaitToFinishDying();

            gameHarness.EnsureGameStatus(GameStatus.Respawning);

            await gameHarness.WaitToRespawn();

            gameHarness.EnsureGameStatus(GameStatus.Alive);

            gameHarness.Game.PacMan.Should().BeEquivalentTo(
                new
                {
                    Location = initialPosition
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
            var pacManLocation = _gameSettings.PacMan.Location;
            var expectedDirection = direction.Opposite();
            
            _gameSettings.PacMan = new PacMan(pacManLocation, expectedDirection);
            _gameSettings.Walls.Add(pacManLocation + direction);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            // Have to use Game.ChangeDirection not gameHarness.ChangeDirection here as 
            // we are expecting the call to ChangeDirection to have no effect.
            await gameHarness.Game.ChangeDirection(direction);

            gameHarness.Game.PacMan.Should().BeEquivalentTo(new
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
            var pacManLocation = _gameSettings.PacMan.Location;
            var expectedDirection = direction.Opposite();

            _gameSettings.PacMan = new PacMan(pacManLocation, expectedDirection);
            _gameSettings.Doors.Add(pacManLocation + direction);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            // Have to use Game.ChangeDirection not gameHarness.ChangeDirection here as 
            // we are expecting the call to ChangeDirection to have no effect.
            await gameHarness.Game.ChangeDirection(direction);

            gameHarness.Game.PacMan.Should().BeEquivalentTo(new
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

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.EatPill();

            var scoreBeforeGhost = gameHarness.Score;

            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Left);

            foreach (var ghost in gameHarness.Game.Ghosts.Values)
            {
                gameHarness.WeExpectThatGhost(ghost).IsAt(ghostStart.Right);
            }

            await gameHarness.EatGhosts(ghosts);

            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Left.Left);

            gameHarness.Game.Should().BeEquivalentTo(new {
                Score = scoreBeforeGhost + totalScore,
                PointsForEatingLastGhost = ghostScore
            });
            
        }
    }
}




