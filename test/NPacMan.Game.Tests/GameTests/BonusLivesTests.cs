using FluentAssertions;
using FluentAssertions.Execution;
using NPacMan.Game.Tests.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class BonusLivesTests
    {
        private static class PointsFor
        {
            public const int EatingCoin = 10;
            public const int EatingPowerPill = 50;
            public const int EatingFirstFruit = 100;
            public const int EatingFirstGhost = 200;
        }

        [Fact]
        public async Task WeGetAnExtraLifeWhenScoreReachesBonusLifePointAfterEatingCoin()
        {
            var gameSettings = new TestGameSettings();
            gameSettings.Coins.Add(gameSettings.PacMan.Location.Below);
            gameSettings.Coins.Add(gameSettings.PacMan.Location.FarAway());
            gameSettings.PointsNeededForBonusLife = PointsFor.EatingCoin;

            var gameHarness = new GameHarness(gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Down);

            var previousLives = gameHarness.Lives;

            using var _ = new AssertionScope();
            await gameHarness.AssertSingleNotificationFires(GameNotification.ExtraPac, async () =>
            {
                await gameHarness.EatCoin();

            });
            gameHarness.Game.Lives.Should().Be(previousLives + 1);
        }

        [Fact]
        public async Task WeGetAnExtraLifeWhenScoreReachesBonusLifePointAfterEatingCoinThatCompletesLevel()
        {
            var gameSettings = new TestGameSettings();
            gameSettings.Coins.Add(gameSettings.PacMan.Location.Below);
            gameSettings.PointsNeededForBonusLife = PointsFor.EatingCoin;

            var gameHarness = new GameHarness(gameSettings);
            gameHarness.StartGame();

            var previousLives = gameHarness.Lives;

            await gameHarness.ChangeDirection(Direction.Down);

            using var _ = new AssertionScope();
            await gameHarness.AssertSingleNotificationFires(GameNotification.ExtraPac, async () =>
            {
                await gameHarness.EatCoin();

            });
            gameHarness.Game.Lives.Should().Be(previousLives + 1);
        }

        [Fact]
        public async Task WeGetAnExtraLifeOnceWhenScoreReachesBonusLifePointAfterScoreIncreasesFromEatingFurtherCoins()
        {
            var gameSettings = new TestGameSettings();
            gameSettings.Coins.Add(gameSettings.PacMan.Location.Below);
            gameSettings.Coins.Add(gameSettings.PacMan.Location.Below.Below);
            gameSettings.Coins.Add(gameSettings.PacMan.Location.FarAway());
            gameSettings.PointsNeededForBonusLife = PointsFor.EatingCoin + 1;

            var gameHarness = new GameHarness(gameSettings);
            gameHarness.StartGame();

            var previousLives = gameHarness.Lives;

            await gameHarness.ChangeDirection(Direction.Down);

            await gameHarness.EatCoin();

            using var _ = new AssertionScope();
            await gameHarness.AssertSingleNotificationFires(GameNotification.ExtraPac, async () =>
            {
                await gameHarness.EatCoin();

            });
            gameHarness.Game.Lives.Should().Be(previousLives + 1);
        }

        [Fact]
        public async Task WeGetAnExtraLifeWhenScoreReachesBonusLifePointAfterEatingPowerPill()
        {
            var gameSettings = new TestGameSettings();
            gameSettings.PowerPills.Add(gameSettings.PacMan.Location.Below);
            gameSettings.PowerPills.Add(gameSettings.PacMan.Location.FarAway());
            gameSettings.PointsNeededForBonusLife = PointsFor.EatingPowerPill;

            var gameHarness = new GameHarness(gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Down);

            var previousLives = gameHarness.Lives;

            using var _ = new AssertionScope();
            await gameHarness.AssertSingleNotificationFires(GameNotification.ExtraPac, async () =>
            {
                await gameHarness.EatPill();
            });

            gameHarness.Game.Lives.Should().Be(previousLives + 1);
        }

        [Fact]
        public async Task WeGetAnExtraLifeWhenScoreReachesBonusLifePointAfterEatingPowerPillThatCompletesLevel()
        {
            var gameSettings = new TestGameSettings();
            gameSettings.PowerPills.Add(gameSettings.PacMan.Location.Below);
            gameSettings.PointsNeededForBonusLife = PointsFor.EatingPowerPill;

            var gameHarness = new GameHarness(gameSettings);
            gameHarness.StartGame();

            var previousLives = gameHarness.Lives;

            await gameHarness.ChangeDirection(Direction.Down);

            using var _ = new AssertionScope();
            await gameHarness.AssertSingleNotificationFires(GameNotification.ExtraPac, async () =>
            {
                await gameHarness.EatPill();
            });

            gameHarness.Game.Lives.Should().Be(previousLives + 1);
        }

        [Fact(DisplayName = "We get an extra life once when score reaches bonus life point after score increases from eating further power pills")]
        public async Task WeGetAnExtraLifeOnceWhenScoreReachesBonusLifePointAfterScoreIncreasesFromEatingFurtherPowerPills()
        {
            var gameSettings = new TestGameSettings();
            gameSettings.PowerPills.Add(gameSettings.PacMan.Location.Below);
            gameSettings.PowerPills.Add(gameSettings.PacMan.Location.Below.Below);
            gameSettings.PowerPills.Add(gameSettings.PacMan.Location.FarAway());
            gameSettings.PointsNeededForBonusLife = PointsFor.EatingPowerPill + 1;

            var gameHarness = new GameHarness(gameSettings);
            gameHarness.StartGame();

            var previousLives = gameHarness.Lives;

            await gameHarness.ChangeDirection(Direction.Down);

            await gameHarness.EatPill();

            using var _ = new AssertionScope();
            await gameHarness.AssertSingleNotificationFires(GameNotification.ExtraPac, async () =>
            {
                await gameHarness.EatPill();
            });

            gameHarness.Game.Lives.Should().Be(previousLives + 1);
        }

        [Fact]
        public async Task WeGetAnExtraLifeWhenScoreReachesBonusLifePointAfterEatingFruit()
        {
            var gameSettings = new TestGameSettings();
            gameSettings.Coins.Add(gameSettings.PacMan.Location.Below);
            gameSettings.Coins.Add(gameSettings.PacMan.Location.FarAway());
            gameSettings.Fruit = gameSettings.PacMan.Location.Below.Below;
            gameSettings.FruitAppearsAfterCoinsEaten.Add(1);
            gameSettings.PointsNeededForBonusLife = PointsFor.EatingFirstFruit;

            var gameHarness = new GameHarness(gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Down);

            var previousLives = gameHarness.Lives;

            await gameHarness.EatCoin();

            gameHarness.WeExpectThatPacMan().HasLives(previousLives);

            using var _ = new AssertionScope();
            await gameHarness.AssertSingleNotificationFires(GameNotification.ExtraPac, async () =>
            {
                await gameHarness.EatFruit();
            });

            gameHarness.Game.Lives.Should().Be(previousLives + 1);
        }

        [Fact]
        public async Task WeGetAnExtraLifeWhenScoreReachesBonusLifePointAfterEatingAGhost()
        {
            var gameSettings = new TestGameSettings();

            var ghostStart = gameSettings.PacMan.Location.Left.Left.Left;
            var ghost = GhostBuilder.New()
                                    .WithLocation(ghostStart)
                                    .WithChaseStrategyRight()
                                    .Create();

            gameSettings.Ghosts.Add(ghost);
            gameSettings.PowerPills.Add(gameSettings.PacMan.Location.FarAway());
            gameSettings.PowerPills.Add(gameSettings.PacMan.Location.Left);
            gameSettings.PointsNeededForBonusLife = PointsFor.EatingFirstGhost;

            var gameHarness = new GameHarness(gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.EatPill();

            var previousLives = gameHarness.Lives;

            using var _ = new AssertionScope();
            await gameHarness.AssertSingleNotificationFires(GameNotification.ExtraPac, async () =>
            {
                await gameHarness.EatGhost(ghost);
            });

            gameHarness.Game.Lives.Should().Be(previousLives + 1);
        }
    }
}