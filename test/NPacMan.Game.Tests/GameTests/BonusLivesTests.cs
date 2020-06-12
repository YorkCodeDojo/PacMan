using FluentAssertions;
using Xunit;
using NPacMan.Game.Tests.Helpers;
using System.Threading.Tasks;
using System;
using FluentAssertions.Execution;

namespace NPacMan.Game.Tests.GameTests
{
    public class BonusLivesTests
    {
        private readonly DateTime _now;
        private readonly TestGameClock _gameClock;
        
        private int _numberOfNotificationsTriggered;

        public BonusLivesTests()
        {
            _now = DateTime.UtcNow;
            _gameClock = new TestGameClock();
            _numberOfNotificationsTriggered = 0;
        }

        private Game CreateInitialGameSettings(Action<TestGameSettings> configure)
        {
            var gameSettings = new TestGameSettings
            {
                PointsNeededForBonusLife = 5
            };
            
            configure(gameSettings);

            return new Game(_gameClock, gameSettings)
                        .Subscribe(GameNotification.ExtraPac, () => _numberOfNotificationsTriggered++);    
        }

        private async Task PlayUntilBonusLife(Game game)
        {
            game.StartGame();

            await game.ChangeDirection(Direction.Down);

            await _gameClock.Tick();
        }

        [Fact]
        public async Task WeGetAnExtraLifeWhenScoreReachesBonusLifePointAfterEatingCoin()
        {
            var game = CreateInitialGameSettings(gameSettings =>
                {
                    gameSettings.Coins.Add(gameSettings.PacMan.Location.Below);
                    gameSettings.Coins.Add(gameSettings.PacMan.Location.FarAway());
                });
            game.StartGame();

            await game.ChangeDirection(Direction.Down);

            var previousLives = game.Lives;

            await _gameClock.Tick();
            
            using var _ = new AssertionScope();
            game.Lives.Should().Be(previousLives + 1);
            _numberOfNotificationsTriggered.Should().Be(1);
        }



        [Fact]
        public async Task WeGetAnExtraLifeWhenScoreReachesBonusLifePointAfterEatingCoinThatCompletesLevel()
        {
            var game = CreateInitialGameSettings(gameSettings =>
                        gameSettings.Coins.Add(gameSettings.PacMan.Location.Below));

            var previousLives = game.Lives;

            await PlayUntilBonusLife(game);

            using var _ = new AssertionScope();
            game.Lives.Should().Be(previousLives + 1);
            _numberOfNotificationsTriggered.Should().Be(1);
        }


        [Fact]
        public async Task WeGetAnExtraLifeOnceWhenScoreReachesBonusLifePointAfterScoreIncreasesFromEatingFurtherCoins()
        {
            var game = CreateInitialGameSettings(gameSettings =>
            {
                gameSettings.Coins.Add(gameSettings.PacMan.Location.Below);
                gameSettings.Coins.Add(gameSettings.PacMan.Location.Below.Below);
                gameSettings.Coins.Add(gameSettings.PacMan.Location.FarAway());
            });
            var previousLives = game.Lives;
            await PlayUntilBonusLife(game);

            await _gameClock.Tick();

            using var _ = new AssertionScope();
            game.Lives.Should().Be(previousLives + 1);
            _numberOfNotificationsTriggered.Should().Be(1);
        }

        [Fact]
        public async Task WeGetAnExtraLifeWhenScoreReachesBonusLifePointAfterEatingPowerPill()
        {
            var game = CreateInitialGameSettings(gameSettings =>
                {
                    gameSettings.PowerPills.Add(gameSettings.PacMan.Location.Below);
                    gameSettings.PowerPills.Add(gameSettings.PacMan.Location.FarAway());
                });
            game.StartGame();

            await game.ChangeDirection(Direction.Down);

            var previousLives = game.Lives;

            await _gameClock.Tick();

              using var _ = new AssertionScope();
            
            game.Lives.Should().Be(previousLives + 1);

            _numberOfNotificationsTriggered.Should().Be(1);
        }

        [Fact]
        public async Task WeGetAnExtraLifeWhenScoreReachesBonusLifePointAfterEatingPowerPillThatCompletesLevel()
        {
            var game = CreateInitialGameSettings(gameSettings =>
                        gameSettings.PowerPills.Add(gameSettings.PacMan.Location.Below));

            var previousLives = game.Lives;

            await PlayUntilBonusLife(game);

            using var _ = new AssertionScope();
            
            game.Lives.Should().Be(previousLives + 1);

            _numberOfNotificationsTriggered.Should().Be(1);
        }

        [Fact(DisplayName = "We get an extra life once when score reaches bonus life point after score increases from eating further power pills")]
        public async Task WeGetAnExtraLifeOnceWhenScoreReachesBonusLifePointAfterScoreIncreasesFromEatingFurtherPowerPills()
        {
            var game = CreateInitialGameSettings(gameSettings =>
            {
                gameSettings.PowerPills.Add(gameSettings.PacMan.Location.Below);
                gameSettings.PowerPills.Add(gameSettings.PacMan.Location.Below.Below);
                gameSettings.PowerPills.Add(gameSettings.PacMan.Location.FarAway());
            });
            var previousLives = game.Lives;
            await PlayUntilBonusLife(game);

            await _gameClock.Tick();
            
            using var _ = new AssertionScope();
            
            game.Lives.Should().Be(previousLives + 1);

            _numberOfNotificationsTriggered.Should().Be(1);
        }

        [Fact]
        public async Task WeGetAnExtraLifeWhenScoreReachesBonusLifePointAfterEatingFruit()
        {
            var game = CreateInitialGameSettings(gameSettings =>
                {
                    gameSettings.Coins.Add(gameSettings.PacMan.Location.Below);
                    gameSettings.Coins.Add(gameSettings.PacMan.Location.FarAway());
                    gameSettings.Fruit = gameSettings.PacMan.Location.Below.Below;
                    gameSettings.FruitAppearsAfterCoinsEaten.Add(1);
                    gameSettings.PointsNeededForBonusLife = 100;
                });

            game.StartGame();

            await game.ChangeDirection(Direction.Down);

            var previousLives = game.Lives;

            await _gameClock.Tick(); // eats coin

            if (game.Lives !=previousLives)
            {
                throw new Exception($"Lives should be {previousLives} not {game.Lives}.");
            }

            await _gameClock.Tick(); // eats fruit

            using var _ = new AssertionScope();
            
            game.Lives.Should().Be(previousLives + 1);

            _numberOfNotificationsTriggered.Should().Be(1);
        }
        
        [Fact]
        public async Task WeGetAnExtraLifeWhenScoreReachesBonusLifePointAfterEatingAGhost()
        {
            var game = CreateInitialGameSettings(gameSettings =>
                {
                    var ghostStart = gameSettings.PacMan.Location.Left.Left.Left;
                    var ghosts = GhostBuilder.New().WithLocation(ghostStart).WithChaseStrategyRight()
                        .Create();
                    
                    gameSettings.Ghosts.Add(ghosts);
                    
                    gameSettings.PowerPills.Add(gameSettings.PacMan.Location.FarAway());
                    gameSettings.PowerPills.Add(gameSettings.PacMan.Location.Left);
                    gameSettings.PointsNeededForBonusLife = 200;
                });
            game.StartGame();

            await game.ChangeDirection(Direction.Left);

            await _gameClock.Tick();

            var previousLives = game.Lives;
            
            await _gameClock.Tick();


            using var _ = new AssertionScope();
            
            game.Lives.Should().Be(previousLives + 1);

            _numberOfNotificationsTriggered.Should().Be(1);
        }
    }
}