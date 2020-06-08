using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using NPacMan.Game.Tests.Helpers;
using Xunit;
using static NPacMan.Game.Tests.Helpers.Ensure;

namespace NPacMan.Game.Tests.GameTests
{
    public class GameTests
    {
        private readonly TestGameSettings _gameSettings;
        private readonly TestGameClock _gameClock;
        private readonly Game _game;

        public GameTests()
        {
            _gameSettings = new TestGameSettings();
            _gameClock = new TestGameClock();
            _game = new Game(_gameClock, _gameSettings);
        }

        [Fact]
        public void GameStartsWithThreeLives()
        {
            _game.Lives.Should().Be(3);
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
        public void CanReadPortalsFromGame()
        {
            _gameSettings.Portals.Add((1,2),(3,4));

            _game.Portals.Should().BeEquivalentTo(new Dictionary<CellLocation, CellLocation> {
                [(1,2)] = (3,4)
            });
        }

        [Fact]
        public void DoorsShouldAlsoBeAGameWall()
        {
            _gameSettings.Doors.Add((1,1));
            _gameSettings.Doors.Add((1,2));
            _gameSettings.Walls.Add((1,3));
            _gameSettings.Walls.Add((1,4));

            var game = new Game(_gameClock, _gameSettings);

            game.Walls.Should().BeEquivalentTo(new CellLocation[] {
                (1,1),
                (1,2),
                (1,3),
                (1,4)
            });
        }

        [Fact]
        public async Task BeforeATickIfProcessedTheGameNotificationShouldFire()
        {
            var numberOfNotificationsTriggered = 0;

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, _gameSettings);
            game.Subscribe(GameNotification.PreTick, () => numberOfNotificationsTriggered++);
            game.StartGame();

            using var _ = new AssertionScope();

            numberOfNotificationsTriggered.Should().Be(0);

            await gameClock.Tick();
            numberOfNotificationsTriggered.Should().Be(1);

            await gameClock.Tick();
            numberOfNotificationsTriggered.Should().Be(2);

            await gameClock.Tick();
            numberOfNotificationsTriggered.Should().Be(3);
        }

        [Fact]
        public async Task EatingLastCoinCompletesLevel()
        {
            var gameClock = new TestGameClock();
            _gameSettings.Ghosts.Add(GhostBuilder.New().WithLocation(_gameSettings.PacMan.Location.FarAway()).Create());
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);
            var game = new Game(gameClock, _gameSettings);
            
            game.StartGame();
            await game.ChangeDirection(Direction.Left);
            await gameClock.Tick();

            game.Should().BeEquivalentTo(new {
                Status = GameStatus.ChangingLevel,
                Level = 1,
                Ghosts = new Dictionary<string, Ghost>()
            });
        }

        [Fact]
        public async Task EatingLastCoinWhenPowerPillExistsDoesntChangeGameStatus()
        {
            var gameClock = new TestGameClock();
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.FarAway());
            var game = new Game(gameClock, _gameSettings);
            
            game.StartGame();
            await game.ChangeDirection(Direction.Left);

            var status = game.Status;
            
            await gameClock.Tick();

            game.Should().BeEquivalentTo(new {
                Status = status,
                Level = 1
            });
        }

        [Fact]
        public async Task EatingLastPillCompletesLevel()
        {
            var gameClock = new TestGameClock();
            _gameSettings.Ghosts.Add(GhostBuilder.New().WithLocation(_gameSettings.PacMan.Location.FarAway()).Create());
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);
            var game = new Game(gameClock, _gameSettings);
            
            game.StartGame();
            await game.ChangeDirection(Direction.Left);
            await gameClock.Tick();

            game.Should().BeEquivalentTo(new {
                Status = GameStatus.ChangingLevel,
                Level = 1,
                Ghosts = new Dictionary<string, Ghost>()
            });
        }

        [Fact]
        public async Task EatingLastPillWhenCoinExistsDoesntChangeGameStatus()
        {
            var gameClock = new TestGameClock();
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.FarAway());
            var game = new Game(gameClock, _gameSettings);
            
            game.StartGame();

            var status = game.Status;
            await game.ChangeDirection(Direction.Left);
            await gameClock.Tick();

            game.Should().BeEquivalentTo(new {
                Status = status,
                Level = 1
            });
        }

        [Fact]
        public void TheScoreIsZeroWhenTheGameStarts()
        {
            var gameClock = new TestGameClock();
            var game = new Game(gameClock, _gameSettings);
            
            game.StartGame();

            game.Should().BeEquivalentTo(new {
                Score = 0
            });
        }

        [Fact]
        public void TheLevelIsOneWhenTheGameStarts()
        {
            var gameClock = new TestGameClock();
            var game = new Game(gameClock, _gameSettings);
            
            game.StartGame();

            game.Should().BeEquivalentTo(new {
                Level = 1
            });
        }

        [Fact]
        public async Task TransitionsFromChangingLevelToNextLevelAfterFourSeconds()
        {
            var now = DateTime.UtcNow;
            var gameClock = new TestGameClock();
            var ghostHomeLocation = _gameSettings.PacMan.Location.FarAway();
            var ghosts = GhostBuilder.New()
                    .WithChaseStrategyRight()
                    .WithLocation(ghostHomeLocation)
                    .CreateMany(3);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left.Left);
            _gameSettings.Ghosts.AddRange(ghosts);
            _gameSettings.FruitAppearsAfterCoinsEaten.Add(1);
            
            var game = new Game(gameClock, _gameSettings);
            
            game.StartGame();
            await game.ChangeDirection(Direction.Left);
            await gameClock.Tick(now);
            await gameClock.Tick(now);

            if (game.Status != GameStatus.ChangingLevel)
            {
                throw new Exception($"Game status should be GameStatus.ChangingLevel not {game.Status}");
            }

            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left.Left);

            await gameClock.Tick(now.AddSeconds(4));

            game.Should().BeEquivalentTo(new {
                Status = GameStatus.Alive,
                Level = 2,
                PacMan = _gameSettings.PacMan,
                Ghosts = ghosts.ToDictionary(x => x.Name, x => new {
                    Location = x.Home,
                    Edible = false
                }),
                Coins = _gameSettings.Coins,
                PowerPills = _gameSettings.PowerPills,
                Fruits = new object[0]
            });
        }
    }
}
