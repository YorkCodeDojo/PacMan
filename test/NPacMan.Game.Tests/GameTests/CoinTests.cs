using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using System;
using System.Threading.Tasks;
using NPacMan.Game.Tests.Helpers;

namespace NPacMan.Game.Tests.GameTests
{
    public class CoinTests
    {
        private readonly TestGameSettings _gameSettings;

        public CoinTests()
        {
            _gameSettings = new TestGameSettings();
        }

        [Fact]
        public async Task ScoreDoesNotChangeWhenNoCoinIsCollected()
        {
            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            var score = gameHarness.Score;

            await gameHarness.ChangeDirection(Direction.Down);

            await gameHarness.Move();

            gameHarness.Score.Should().Be(score);
        }

        [Fact]
        public async Task IncrementsScoreBy10WhenCoinCollected()
        {
            var pacManStartingLocation = _gameSettings.PacMan.Location;
            _gameSettings.Coins.Add(pacManStartingLocation.Below);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame(); 

            await gameHarness.ChangeDirection(Direction.Down);

            await gameHarness.EatCoin();

            gameHarness.Score.Should().Be(10);
        }

        [Fact]
        public async Task CannotCollectTheSameCoinTwice()
        {
            var pacManStartingLocation = _gameSettings.PacMan.Location;
            _gameSettings.Coins.Add(pacManStartingLocation.Below);
            _gameSettings.Coins.Add(pacManStartingLocation.FarAway());

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Down);
            await gameHarness.EatCoin();

            await gameHarness.ChangeDirection(Direction.Up);
            await gameHarness.Move();

            await gameHarness.ChangeDirection(Direction.Down);
            await gameHarness.Move();

            gameHarness.Score.Should().Be(10);
        }

        [Fact]
        public async Task IncrementsScoreBy20WhenTwoCoinsAreCollected()
        {
            var pacManStartingLocation = _gameSettings.PacMan.Location;
            _gameSettings.Coins.Add(pacManStartingLocation.Below);
            _gameSettings.Coins.Add(pacManStartingLocation.Below.Below);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Down);

            await gameHarness.EatCoin();
            await gameHarness.EatCoin();

            gameHarness.Score.Should().Be(20);
        }

        [Fact]
        public async Task CoinShouldBeCollected()
        {
            var coinLocation = _gameSettings.PacMan.Location.Below;
            _gameSettings.Coins.Add(coinLocation);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Down);
            await gameHarness.EatCoin();

            gameHarness.Game.Coins.Should().NotContain(coinLocation);
        }

        [Fact]
        public async Task JustTheCollectedCoinShouldBeCollected()
        {
            var pacManStartingLocation = _gameSettings.PacMan.Location;
            _gameSettings.Coins.Add(pacManStartingLocation.Below);
            _gameSettings.Coins.Add(pacManStartingLocation.Below.Below);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Down);

            await gameHarness.EatCoin();

            gameHarness.Game.Coins.Should().NotContain(pacManStartingLocation.Below);
            gameHarness.Game.Coins.Should().Contain(pacManStartingLocation.Below.Below);
        }

        [Fact]
        public async Task GameContainsAllCoins()
        {
            var coin1 = _gameSettings.PacMan.Location.FarAway().Left;
            var coin2 = _gameSettings.PacMan.Location.FarAway();
            var coin3 = _gameSettings.PacMan.Location.FarAway().Right;

            _gameSettings.Coins.Add(coin1);
            _gameSettings.Coins.Add(coin2);
            _gameSettings.Coins.Add(coin3);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();

            await gameHarness.Move();

            gameHarness.Game.Coins.Should().BeEquivalentTo(
                coin1,
                coin2,
                coin3
            );
        }

        [Fact]
        public async Task PacManDoesNotCollectCoinAndScoreStaysTheSameWhenCollidesWithGhost()
        {
            var ghostAndCoinLocation = _gameSettings.PacMan.Location + Direction.Right;

            var ghost = GhostBuilder.New()
                .WithLocation(ghostAndCoinLocation)
                .Create();

            _gameSettings.Ghosts.Add(ghost);
            _gameSettings.Coins.Add(ghostAndCoinLocation);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();
            var score = gameHarness.Score;

            await gameHarness.ChangeDirection(Direction.Right);
            await gameHarness.GetEatenByGhost(ghost);

            using var _ = new AssertionScope();
            gameHarness.Game.Coins.Should().ContainEquivalentOf(ghostAndCoinLocation);
            gameHarness.Score.Should().Be(score);
        }

        [Fact]
        public async Task WhenPacManEatsACoinTheGameNotificationShouldFire()
        {
            var pacManStartingLocation = _gameSettings.PacMan.Location;
            _gameSettings.Coins.Add(pacManStartingLocation.Below);

            var numberOfNotificationsTriggered = 0;

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.Game.Subscribe(GameNotification.EatCoin, () => numberOfNotificationsTriggered++);
            gameHarness.StartGame(); 

            await gameHarness.ChangeDirection(Direction.Down);

            await gameHarness.EatCoin();

            numberOfNotificationsTriggered.Should().Be(1);
        }
    }
}
