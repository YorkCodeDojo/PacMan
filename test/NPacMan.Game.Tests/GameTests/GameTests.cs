using FluentAssertions;
using FluentAssertions.Execution;
using NPacMan.Game.Tests.GhostStrategiesForTests;
using NPacMan.Game.Tests.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class GameTests
    {
        private readonly TestGameSettings _gameSettings;

        public GameTests()
        {
            _gameSettings = new TestGameSettings();
        }

        [Fact]
        public void GameStartsWithThreeLives()
        {
            var gameHarness = new GameHarness(_gameSettings);

            gameHarness.Game.Lives.Should().Be(3);
        }

        [Fact]
        public void TheGameCanReadTheWidthFromTheBoard()
        {
            var gameBoardWidth = 100;
            _gameSettings.Width = gameBoardWidth;

            var gameHarness = new GameHarness(_gameSettings);

            gameHarness.Game.Width.Should().Be(gameBoardWidth);
        }

        [Fact]
        public void TheGameCanReadTheHeightFromTheBoard()
        {
            var gameBoardHeight = 100;
            _gameSettings.Height = gameBoardHeight;

            var gameHarness = new GameHarness(_gameSettings);

            gameHarness.Game.Height.Should().Be(gameBoardHeight);
        }

        [Fact]
        public void CanReadPortalsFromGame()
        {
            _gameSettings.Portals.Add((1, 2), (3, 4));

            var gameHarness = new GameHarness(_gameSettings);

            gameHarness.Game.Portals.Should().BeEquivalentTo(new Dictionary<CellLocation, CellLocation>
            {
                [(1, 2)] = (3, 4)
            });
        }

        [Fact]
        public void DoorsShouldAlsoBeAGameWall()
        {
            _gameSettings.Doors.Add((1, 1));
            _gameSettings.Doors.Add((1, 2));
            _gameSettings.Walls.Add((1, 3));
            _gameSettings.Walls.Add((1, 4));

            var gameHarness = new GameHarness(_gameSettings);

            gameHarness.Game.WallsAndDoors.Should().BeEquivalentTo(new CellLocation[] {
                (1,1),
                (1,2),
                (1,3),
                (1,4)
            });
        }

        [Fact]
        public void WallsShouldNotContainDoors()
        {
            _gameSettings.Doors.Add((1, 1));
            _gameSettings.Doors.Add((1, 2));
            _gameSettings.Walls.Add((1, 3));
            _gameSettings.Walls.Add((1, 4));

            var gameHarness = new GameHarness(_gameSettings);

            gameHarness.Game.Walls.Should().BeEquivalentTo(new CellLocation[] {
                (1,3),
                (1,4)
            });
        }

        [Fact]
        public async Task BeforeATickIfProcessedTheGameNotificationShouldFire()
        {
            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            using var _ = new AssertionScope();

            await gameHarness.AssertSingleNotificationFires(GameNotification.PreTick, async () =>
            {
                await gameHarness.Move();
            });

            await gameHarness.AssertSingleNotificationFires(GameNotification.PreTick, async () =>
            {
                await gameHarness.Move();
            });

            await gameHarness.AssertSingleNotificationFires(GameNotification.PreTick, async () =>
            {
                await gameHarness.Move();
            });

            await gameHarness.AssertSingleNotificationFires(GameNotification.PreTick, async () =>
            {
                await gameHarness.Move();
            });
        }

        [Fact]
        public async Task EatingLastCoinCompletesLevel()
        {
            _gameSettings.Ghosts.Add(GhostBuilder.New().WithLocation(_gameSettings.PacMan.Location.FarAway()).Create());
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);
            await gameHarness.EatCoin();

            gameHarness.Game.Should().BeEquivalentTo(new
            {
                Status = GameStatus.ChangingLevel,
                Level = 1,
                Ghosts = new Dictionary<string, Ghost>()
            });
        }

        [Fact]
        public async Task EatingLastCoinWhenPowerPillExistsDoesntChangeGameStatus()
        {
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.FarAway());

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);

            var status = gameHarness.Status;

            await gameHarness.EatCoin();

            gameHarness.Game.Should().BeEquivalentTo(new
            {
                Status = status,
                Level = 1
            });
        }

        [Fact]
        public async Task EatingLastPillCompletesLevel()
        {
            _gameSettings.Ghosts.Add(GhostBuilder.New().WithLocation(_gameSettings.PacMan.Location.FarAway()).Create());
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);
            await gameHarness.EatPill();

            gameHarness.Game.Should().BeEquivalentTo(new
            {
                Status = GameStatus.ChangingLevel,
                Level = 1,
                Ghosts = new Dictionary<string, Ghost>()
            });
        }

        [Fact]
        public async Task EatingLastPillWhenCoinExistsDoesntChangeGameStatus()
        {
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.FarAway());

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            var status = gameHarness.Game.Status;
            await gameHarness.ChangeDirection(Direction.Left);
            await gameHarness.EatPill();

            gameHarness.Game.Should().BeEquivalentTo(new
            {
                Status = status,
                Level = 1
            });
        }

        [Fact]
        public async Task TheScoreIsZeroWhenTheGameStarts()
        {
            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            gameHarness.Game.Should().BeEquivalentTo(new
            {
                Score = 0
            });
        }

        [Fact]
        public async Task TheLevelIsOneWhenTheGameStarts()
        {
            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            gameHarness.Game.Should().BeEquivalentTo(new
            {
                Level = 1
            });
        }

        [Fact]
        public async Task TransitionsFromChangingLevelToNextLevelAfterFourSeconds()
        {
            var ghostHomeLocation = _gameSettings.PacMan.Location.FarAway();
            var ghosts = GhostBuilder.New()
                    .WithChaseStrategyRight()
                    .WithLocation(ghostHomeLocation)
                    .CreateMany(3);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left.Left);
            _gameSettings.Ghosts.AddRange(ghosts);
            _gameSettings.FruitAppearsAfterCoinsEaten.Add(1);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);
            await gameHarness.EatCoin();
            await gameHarness.EatPill();

            gameHarness.EnsureGameStatus(GameStatus.ChangingLevel);

            await gameHarness.WaitForEndOfLevelFlashingToComplete();

            gameHarness.Game.Should().BeEquivalentTo(new
            {
                Status = GameStatus.Alive,
                Level = 2,
                PacMan = _gameSettings.PacMan,
                Ghosts = ghosts.ToDictionary(x => x.Name, x => new
                {
                    Location = x.Home,
                    Edible = false
                }),
                Coins = _gameSettings.Coins,
                PowerPills = _gameSettings.PowerPills,
                Fruits = new object[0]
            });
        }

        [Fact]
        public async Task GameMovesStraightIntoAttactMode()
        {
            _gameSettings.InitialGameStatus = GameStatus.Initial;
            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.NOP();

            gameHarness.Game.Status.Should().Be(GameStatus.AttractMode);
        }

        [Fact]
        public async Task GameMovesToAliveWhenUserPressesStart()
        {
            _gameSettings.InitialGameStatus = GameStatus.Initial;
            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();
            await gameHarness.WaitAndEnterAttractMode();
            await gameHarness.PressStart();

            gameHarness.Game.Status.Should().Be(GameStatus.Alive);
        }

        [Fact]
        public async Task GameShouldBeInAttractModeWhenNoLivesLeft()
        {
            _gameSettings.InitialLives = 1;
            var ghost = GhostBuilder.New()
                                    .WithLocation(_gameSettings.PacMan.Location.Left.Left.Left)
                                    .WithScatterStrategyRight()
                                    .WithChaseStrategyRight()
                                    .Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);
            await gameHarness.Move();
            await gameHarness.GetEatenByGhost(ghost);

            await gameHarness.WaitAndEnterAttractMode();

            gameHarness.Game.Status.Should().Be(GameStatus.AttractMode);
        }

        [Fact]
        public async Task AfterGameOverAndStartingNewGameAllStateIsRestored()
        {
            var ghostHomeLocation = _gameSettings.PacMan.Location.FarAway();
            var ghosts = GhostBuilder.New()
                    .WithScatterStrategyRight()
                    .WithChaseStrategyRight()
                    .WithLocation(ghostHomeLocation)
                    .CreateMany(3);
            var killerGhost = GhostBuilder.New()
                    .WithLocation(_gameSettings.PacMan.Location.Left.Left.Left.Left.Left.Left)
                    .WithDirection(Direction.Left)
                    .WithScatterTarget(_gameSettings.PacMan.Location.Right)
                    .Create();
            _gameSettings.DirectionPicker = new TestDirectionPicker()
            {
                DefaultDirection = Direction.Left
            };
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.FarAway());
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left.Left);
            _gameSettings.Ghosts.AddRange(ghosts);
            _gameSettings.Ghosts.Add(killerGhost);
            _gameSettings.FruitAppearsAfterCoinsEaten.Add(1);
            _gameSettings.InitialLives = 1;

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);
            await gameHarness.EatCoin();
            await gameHarness.EatPill();

            await gameHarness.ChangeDirection(Direction.Up);
            await gameHarness.WaitForFrightenedTimeToComplete();

            await gameHarness.GetEatenByGhost(killerGhost);
            await gameHarness.WaitAndEnterAttractMode();

            await gameHarness.PressStart();

            gameHarness.Game.Should().BeEquivalentTo(new
            {
                Status = GameStatus.Alive,
                Score = 0,
                Level = 1,
                Lives = 1,
                PacMan = _gameSettings.PacMan,
                Ghosts = ghosts.Concat(new[] { killerGhost }).ToDictionary(x => x.Name, x => new
                {
                    Location = x.Home,
                    Edible = false
                }),
                Coins = _gameSettings.Coins,
                PowerPills = _gameSettings.PowerPills,
                Fruits = new object[0]
            });
        }

        [Fact]
        public async Task AfterGameOverAndStartingNewGameThenBeginningGameNotificationIsPublished()
        {
            var killerGhost = GhostBuilder.New()
                    .WithLocation(_gameSettings.PacMan.Location.Left)
                    .WithScatterStrategy(new StandingStillGhostStrategy())
                    .Create();
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.FarAway());
            _gameSettings.InitialLives = 1;
            _gameSettings.Ghosts.Add(killerGhost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.GetEatenByGhost(killerGhost);
            gameHarness.EnsureGameStatus(GameStatus.Dying);

            await gameHarness.WaitAndEnterAttractMode();

            gameHarness.EnsureGameStatus(GameStatus.AttractMode);

            await gameHarness.AssertSingleNotificationFires(GameNotification.Beginning, async () =>
            {
                await gameHarness.PressStart();
            });
        }

        [Fact]
        public async Task WhenStartingANewGameThenBonusLifeCanBeCollectedAgain()
        {
            var killerGhost = GhostBuilder.New()
                    .WithLocation(_gameSettings.PacMan.Location.Left.Left)
                    .WithScatterStrategy(new StandingStillGhostStrategy())
                    .Create();
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.FarAway());
            _gameSettings.InitialLives = 0;
            _gameSettings.PointsNeededForBonusLife = 1;
            _gameSettings.Ghosts.Add(killerGhost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);
            await gameHarness.EatCoin(); // Awards bonus life

            gameHarness.WeExpectThatPacMan().HasLives(_gameSettings.InitialLives + 1);

            await gameHarness.GetEatenByGhost(killerGhost);
            gameHarness.EnsureGameStatus(GameStatus.Dying);

            await gameHarness.WaitAndEnterAttractMode();

            gameHarness.EnsureGameStatus(GameStatus.AttractMode);

            await gameHarness.PressStart();

            await gameHarness.ChangeDirection(Direction.Left);
            await gameHarness.EatCoin(); // Awards bonus life

            gameHarness.Lives.Should().Be(_gameSettings.InitialLives + 1);
        }

        [Fact]
        public async Task BeginningGameNotificationIsPublishedWhenGameMovesToAliveWhenUserPressesStart()
        {
            _gameSettings.InitialGameStatus = GameStatus.Initial;

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.NOP();

            gameHarness.EnsureGameStatus(GameStatus.AttractMode);

            await gameHarness.AssertSingleNotificationFires(GameNotification.Beginning, async () =>
            {
                await gameHarness.PressStart();
            });

            gameHarness.Game.Status.Should().Be(GameStatus.Alive);
        }

    }
}
