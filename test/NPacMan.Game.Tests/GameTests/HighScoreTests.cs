using FluentAssertions;
using FluentAssertions.Execution;
using NPacMan.Game.GhostStrategies;
using NPacMan.Game.Tests.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class HighScoreTests : IHighScoreStorage
    {
        [Fact]
        public async Task TheHighScoreAlwaysIncrementsAfterEatingACoinDuringTheFirstGame()
        {
            var gameSettings = new TestGameSettings();
            gameSettings.Coins.Add(gameSettings.PacMan.Location.Below);
            gameSettings.Coins.Add(gameSettings.PacMan.Location.FarAway());

            var gameHarness = new GameHarness(gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Down);

            await gameHarness.EatCoin();
      
            gameHarness.Game.HighScore.Should().Be(gameHarness.Score);
        }

        
        [Fact]
        public async Task TheHighScoreAlwaysIncrementsAfterEatingAPowerPillDuringTheFirstGame()
        {
            var gameSettings = new TestGameSettings();
            gameSettings.PowerPills.Add(gameSettings.PacMan.Location.Below);
            gameSettings.PowerPills.Add(gameSettings.PacMan.Location.FarAway());

            var gameHarness = new GameHarness(gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Down);

            await gameHarness.EatPill();
            
            gameHarness.Game.HighScore.Should().Be(gameHarness.Score);
        }

        [Fact]
        public async Task TheHighScoreAlwaysIncrementsAfterEatingFruitDuringTheFirstGame()
        {
            var gameSettings = new TestGameSettings();
            gameSettings.Coins.Add(gameSettings.PacMan.Location.Below);
            gameSettings.Coins.Add(gameSettings.PacMan.Location.FarAway());
            gameSettings.Fruit = gameSettings.PacMan.Location.Below.Below;
            gameSettings.FruitAppearsAfterCoinsEaten.Add(1);

            var gameHarness = new GameHarness(gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Down);

            await gameHarness.EatCoin();

            await gameHarness.EatFruit();

            gameHarness.Game.HighScore.Should().Be(gameHarness.Score);
        }
        
        [Fact]
        public async Task TheHighScoreAlwaysIncrementsAfterEatingAGhostDuringTheFirstGame()
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

            var gameHarness = new GameHarness(gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.EatPill();
            await gameHarness.EatGhost(ghost);

             gameHarness.Game.HighScore.Should().Be(gameHarness.Score);
        }


        [Fact]
        public async Task TheHighScoreDoesNotIncrementAfterEatingACoinIfTheScoreIsNotHigherThanTheCurrentHighScore()
        {
            const int initialHighScore = 1000;
            HighScore = initialHighScore;
            var gameSettings = new TestGameSettings();
            gameSettings.HighScoreStorage = this;
            gameSettings.Coins.Add(gameSettings.PacMan.Location.Below);
            gameSettings.Coins.Add(gameSettings.PacMan.Location.FarAway());

            var gameHarness = new GameHarness(gameSettings);
            gameHarness.StartGame();

            await gameHarness.ChangeDirection(Direction.Down);

            if (gameHarness.Game.HighScore != initialHighScore)
            {
                throw new Exception($"The current high score should be {initialHighScore} not {gameHarness.Game.HighScore}.");
            }

            await gameHarness.EatCoin();
      
            gameHarness.Game.HighScore.Should().Be(initialHighScore);
        }
        private int HighScore = 0;
        public int GetHighScore() => HighScore;
    }
}