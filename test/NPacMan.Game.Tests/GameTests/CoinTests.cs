using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using System;
using System.Threading.Tasks;
using NPacMan.Game.Tests.GhostStrategiesForTests;
using static NPacMan.Game.Tests.Helpers.Ensure;
using NPacMan.Game.Tests.Helpers;

namespace NPacMan.Game.Tests.GameTests
{
    public class CoinTests
    {
        private readonly TestGameSettings _gameSettings;
        private readonly TestGameClock _gameClock;

        public CoinTests()
        {
            _gameSettings = new TestGameSettings();
            _gameClock = new TestGameClock();
        }

        [Fact]
        public async Task ScoreDoesNotChangeWhenNoCoinIsCollected()
        {
            var game = new Game(_gameClock, _gameSettings);
            var score = game.Score;

            await game.ChangeDirection(Direction.Down);

            await _gameClock.Tick();

            game.Score.Should().Be(score);
        }

        [Fact]
        public async Task IncrementsScoreBy10WhenCoinCollected()
        {
            var pacManStartingLocation = _gameSettings.PacMan.Location;
            _gameSettings.Coins.Add(pacManStartingLocation.Below);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame(); 

            await game.ChangeDirection(Direction.Down);

            await _gameClock.Tick();

            game.Score.Should().Be(10);
        }

        [Fact]
        public async Task CannotCollectTheSameCoinTwice()
        {
            var pacManStartingLocation = _gameSettings.PacMan.Location;
            _gameSettings.Coins.Add(pacManStartingLocation.Below);
            _gameSettings.Coins.Add(pacManStartingLocation.FarAway());

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Down);

            await _gameClock.Tick();

            if (game.Score != 10)
                throw new Exception($"Score should be 10 not {game.Score}");

            await game.ChangeDirection(Direction.Up);
            await _gameClock.Tick();

            if (game.Score != 10)
                throw new Exception($"Score should still be 10 not {game.Score}");

            await game.ChangeDirection(Direction.Down);
            await _gameClock.Tick();

            game.Score.Should().Be(10);
        }


        [Fact]
        public async Task IncrementsScoreBy20WhenTwoCoinsAreCollected()
        {
            var pacManStartingLocation = _gameSettings.PacMan.Location;
            _gameSettings.Coins.Add(pacManStartingLocation.Below);
            _gameSettings.Coins.Add(pacManStartingLocation.Below.Below);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame(); 

            await game.ChangeDirection(Direction.Down);

            await _gameClock.Tick();
            await _gameClock.Tick();

            game.Score.Should().Be(20);
        }

        [Fact]
        public async Task CoinShouldBeCollected()
        {
            var game = new Game(_gameClock, _gameSettings);
            game.StartGame(); 
            var (x, y) = game.PacMan.Location;

            await game.ChangeDirection(Direction.Down);

            _gameSettings.Coins.Add((x, y + 1));

            await _gameClock.Tick();

            game.Coins.Should().NotContain((x, y + 1));
        }

        [Fact]
        public async Task JustTheCollectedCoinShouldBeCollected()
        {
            var pacManStartingLocation = _gameSettings.PacMan.Location;
            _gameSettings.Coins.Add(pacManStartingLocation.Below);
            _gameSettings.Coins.Add(pacManStartingLocation.Below.Below);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame(); 

            await game.ChangeDirection(Direction.Down);

            await _gameClock.Tick();

            game.Coins.Should().NotContain(pacManStartingLocation.Below);
            game.Coins.Should().Contain(pacManStartingLocation.Below.Below);
        }

        [Fact]
        public async Task GameContainsAllCoins()
        {
            var gameBoard = new TestGameSettings();
            gameBoard.Coins.Add((1, 1));
            gameBoard.Coins.Add((1, 2));
            gameBoard.Coins.Add((2, 2));

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, gameBoard);
            game.StartGame();
            await gameClock.Tick();

            game.Coins.Should().BeEquivalentTo(
                new CellLocation(1, 1),
                new CellLocation(1, 2),
                new CellLocation(2, 2)
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

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame(); 
            var score = game.Score;

            await game.ChangeDirection(Direction.Right);
            await _gameClock.Tick();

            using var _ = new AssertionScope();
            game.Coins.Should().ContainEquivalentOf(ghostAndCoinLocation);
            game.Score.Should().Be(score);
        }

        [Fact]
        public async Task WhenPacManEatsACoinTheGameNotificationShouldFire()
        {
            var pacManStartingLocation = _gameSettings.PacMan.Location;
            _gameSettings.Coins.Add(pacManStartingLocation.Below);

            var numberOfNotificationsTriggered = 0;

            var game = new Game(_gameClock, _gameSettings);
            game.Subscribe(GameNotification.EatCoin, () => numberOfNotificationsTriggered++);
            game.StartGame(); 

            await game.ChangeDirection(Direction.Down);

            await _gameClock.Tick();

            numberOfNotificationsTriggered.Should().Be(1);
        }
    }
}
