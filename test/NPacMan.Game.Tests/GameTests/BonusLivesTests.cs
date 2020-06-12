using FluentAssertions;
using Xunit;
using NPacMan.Game.Tests.Helpers;
using System.Threading.Tasks;
using System;

namespace NPacMan.Game.Tests.GameTests
{
    public class BonusLivesTests
    {
        private readonly DateTime _now;
        private readonly TestGameClock _gameClock;

        public BonusLivesTests()
        {
            _now = DateTime.UtcNow;
            _gameClock = new TestGameClock();
        }

        [Fact]
        public async Task LivesIncreaseWhenScoreReachesBonusLifePointAfterEatingCoin()
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

            game.Lives.Should().Be(previousLives + 1);
        }

        private Game CreateInitialGameSettings(Action<TestGameSettings> configure)
        {
            var gameSettings = new TestGameSettings();            

            gameSettings.PointsNeededForBonusLife = 5;

            configure(gameSettings);

            return new Game(_gameClock, gameSettings);
        }

        [Fact]
        public async Task LivesIncreaseWhenScoreReachesBonusLifePointAfterEatingCoinThatCompletesLevel()
        {
            var game = CreateInitialGameSettings(gameSettings =>
                        gameSettings.Coins.Add(gameSettings.PacMan.Location.Below));
                        
            var previousLives = game.Lives;

            await PlayUntilBonusLife(game);

            game.Lives.Should().Be(previousLives + 1);
        }

        private async Task PlayUntilBonusLife(Game game)
        {
             game.StartGame();

            await game.ChangeDirection(Direction.Down);

            await _gameClock.Tick();
        }

        [Fact]
        public async Task LivesIncreaseOnceWhenScoreReachesBonusLifePointAfterScoreIncreasesFurther()
        {
            var game = CreateInitialGameSettings(gameSettings => {
                gameSettings.Coins.Add(gameSettings.PacMan.Location.Below);
                gameSettings.Coins.Add(gameSettings.PacMan.Location.Below.Below);
                gameSettings.Coins.Add(gameSettings.PacMan.Location.FarAway());
            });
            var previousLives = game.Lives;
            await PlayUntilBonusLife(game);

            await _gameClock.Tick();

            game.Lives.Should().Be(previousLives + 1);
        }
    }
}