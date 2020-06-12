using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using System.Linq;
using NPacMan.Game.Tests.Helpers;
using System.Threading.Tasks;
using System;
using static NPacMan.Game.Tests.Helpers.Ensure;

namespace NPacMan.Game.Tests.GameTests
{
    public class BonusLivesTests
    {

        private readonly DateTime _now;
        private readonly TestGameSettings _gameSettings;
        private readonly TestGameClock _gameClock;

        public BonusLivesTests()
        {
            _now = DateTime.UtcNow;

            _gameSettings = new TestGameSettings();
            _gameClock = new TestGameClock();
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(2, 2)]
        public async Task LivesIncreaseWhenScoreReachesBonusLifePointAfterEatingCoin(int numberOfCoins, int numberOfTicks)
        {
            var pacManStartingLocation = _gameSettings.PacMan.Location;
            for (int c = 1; c <= numberOfCoins; c++)
            {
                _gameSettings.Coins.Add((pacManStartingLocation.X, pacManStartingLocation.Y + c));
            }

            _gameSettings.PointsNeededForBonusLife = 5;

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Down);

            var previousLives = game.Lives;

            for(int t = 0; t < numberOfTicks; t++)
            {
                await _gameClock.Tick();
            }

            game.Lives.Should().Be(previousLives + 1);
        }
    }
}