using FluentAssertions;
using Xunit;
using System.Linq;
using NPacMan.Game.Tests.Helpers;
using System.Threading.Tasks;
using System;

namespace NPacMan.Game.Tests.GameTests
{
    public class FruitTests
    {
        private readonly TestGameSettings _gameSettings;

        public FruitTests()
        {
            _gameSettings = new TestGameSettings();
        }

        [Fact]
        public async Task FruitShouldNotBeVisibleWhenStartingGame()
        {
            var fruitLocation = _gameSettings.PacMan.Location.FarAway();
            _gameSettings.Fruit = fruitLocation;

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            gameHarness.Game.Fruits.Should().BeEmpty();
        }

        [Fact]
        public async Task FruitShouldFirstAppearAfterSetNumberOfPills()
        {
            var fruitLocation = _gameSettings.PacMan.Location.FarAway();
            _gameSettings.Fruit = fruitLocation;
            _gameSettings.FruitAppearsAfterCoinsEaten.Add(5);

            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left.Left.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left.Left.Left.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left.Left.Left.Left.Left);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);
            await gameHarness.EatCoin();
            await gameHarness.EatCoin();
            await gameHarness.EatCoin();
            await gameHarness.EatCoin();
            await gameHarness.EatCoin();

            gameHarness.Game.Fruits.Should().BeEquivalentTo(new
            {
                Location = _gameSettings.Fruit,
                Type = FruitType.Cherry
            });
        }

        [Fact]
        public async Task FruitShouldDisappearAfterAConfiguredAmountOfTime()
        {
            var fruitLocation = _gameSettings.PacMan.Location.FarAway();
            var gameHarness = await PlayGameUntilFruitAppears(fruitLocation);

            await gameHarness.WaitForFruitToDisappear();

            gameHarness.Game.Fruits.Should().BeEmpty();
        }

        private async Task<GameHarness> PlayGameUntilFruitAppears(CellLocation fruitLocation)
        {
            _gameSettings.Fruit = fruitLocation;
            _gameSettings.FruitAppearsAfterCoinsEaten.Add(2);

            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.FarAway());

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();
            await gameHarness.ChangeDirection(Direction.Left);
            await gameHarness.EatCoin();
            await gameHarness.EatCoin();

            if (!gameHarness.Game.Fruits.Any())
            {
                throw new Exception("Fruit should be visible");
            }

            return gameHarness;
        }

        [Fact]
        public async Task PacManCanEatAFruit()
        {
            var fruitLocation = _gameSettings.PacMan.Location.Left.Left.Left;
            var gameHarness = await PlayGameUntilFruitAppears(fruitLocation);

            var score = gameHarness.Score;
            await gameHarness.EatFruit();

            gameHarness.Game.Should().BeEquivalentTo(new
            {
                Fruits = new Fruit[0],
                Score = score + 100
            });
        }

        [Fact]
        public async Task NotificationsAreRaisedWhenFruitsAreEaten()
        {
            var gameHarness = await PlayGameUntilFruitAppears(_gameSettings.PacMan.Location.Left.Left.Left);

            await gameHarness.AssertSingleNotificationFires(GameNotification.EatFruit, async () =>
            {
                await gameHarness.EatFruit();
            });
        }

        [Theory]
        [InlineData(1, FruitType.Cherry)]
        [InlineData(2, FruitType.Strawberry)]
        [InlineData(3, FruitType.Orange)]
        [InlineData(4, FruitType.Orange)]
        [InlineData(5, FruitType.Bell)]
        [InlineData(6, FruitType.Bell)]
        [InlineData(7, FruitType.Apple)]
        [InlineData(8, FruitType.Apple)]
        [InlineData(9, FruitType.Grapes)]
        [InlineData(10, FruitType.Grapes)]
        [InlineData(11, FruitType.Arcadian)]
        [InlineData(12, FruitType.Arcadian)]
        [InlineData(13, FruitType.Key)]
        [InlineData(14, FruitType.Key)]
        [InlineData(15, FruitType.Key)]
        [InlineData(256, FruitType.Key)]
        public async Task FruitAppearsOnNextLevel(int levelNumber, FruitType expectedFruit)
        {
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left.Left);
            _gameSettings.FruitAppearsAfterCoinsEaten.Add(1);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            for (var level = 0; level < levelNumber - 1; level++)
            {
                await gameHarness.ChangeDirection(Direction.Left);
                await gameHarness.EatCoin();  //Fruit appears

                await gameHarness.EatCoin();  //Level completed

                gameHarness.EnsureGameStatus(GameStatus.ChangingLevel);

                gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Left.Left);

                await gameHarness.WaitForEndOfLevelFlashingToComplete();

                gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location);
            }

            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.EatCoin(); // Eat coin, fruit appears

            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Left);

            gameHarness.Game.Should().BeEquivalentTo(new
            {
                Level = levelNumber,
                Fruits = new[]{
                    new Fruit(_gameSettings.Fruit, expectedFruit)
                }
            });
        }

        [Theory]
        [InlineData(1, 100)]
        [InlineData(2, 300)]
        [InlineData(3, 500)]
        [InlineData(4, 500)]
        [InlineData(5, 700)]
        [InlineData(6, 700)]
        [InlineData(7, 1000)]
        [InlineData(8, 1000)]
        [InlineData(9, 2000)]
        [InlineData(10, 2000)]
        [InlineData(11, 3000)]
        [InlineData(12, 3000)]
        [InlineData(13, 5000)]
        [InlineData(14, 5000)]
        [InlineData(15, 5000)]
        [InlineData(256, 5000)]
        public async Task EatingFruitOnEveryLevel(int levelNumber, int scoreIncrement)
        {
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left.Left);
            _gameSettings.Fruit = _gameSettings.PacMan.Location.Left.Above;
            _gameSettings.FruitAppearsAfterCoinsEaten.Add(1);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            for (var level = 0; level < levelNumber - 1; level++)
            {
                await gameHarness.ChangeDirection(Direction.Left);
                await gameHarness.EatCoin();  //Fruit appears

                await gameHarness.EatCoin();  //Level completed

                gameHarness.EnsureGameStatus(GameStatus.ChangingLevel);

                gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Left.Left);

                await gameHarness.WaitForEndOfLevelFlashingToComplete();

                gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location);
            }

            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.EatCoin(); // Fruit appears

            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Left);

            await gameHarness.ChangeDirection(Direction.Up);

            var score = gameHarness.Score;

            await gameHarness.EatFruit();

            gameHarness.Game.Should().BeEquivalentTo(new
            {
                Level = levelNumber,
                Score = score + scoreIncrement
            });
        }
    }
}