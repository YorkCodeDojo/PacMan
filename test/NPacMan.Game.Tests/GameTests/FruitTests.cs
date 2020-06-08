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
    public class FruitTests
    {
        private readonly DateTime _now;
        private readonly TestGameSettings _gameSettings;
        private readonly TestGameClock _gameClock;

        public FruitTests()
        {
            _now = DateTime.UtcNow;

            _gameSettings = new TestGameSettings();
            _gameClock = new TestGameClock();
        }


        [Fact]
        public void FruitShouldNotBeVisibleWhenStartingGame()
        {
            var fruitLocation = _gameSettings.PacMan.Location.FarAway();
            _gameSettings.Fruit = fruitLocation;
            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            game.Fruits.Should().BeEmpty();            
        }
        
        [Fact]
        public async Task FruitShouldFirstAppearAfterSetNumberOfPills()
        {
            var fruitLocation = _gameSettings.PacMan.Location.FarAway();
            _gameSettings.Fruit = fruitLocation;
            _gameSettings.FruitAppearsAfterCoinsEaten.Add(5);

            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);
            for(var i =1; i<=5; i++)
            {
                _gameSettings.Coins.Add(_gameSettings.Coins[i - 1].Left);
            }
            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
            await game.ChangeDirection(Direction.Left);
            await _gameClock.Tick();
            await _gameClock.Tick();
            await _gameClock.Tick();
            await _gameClock.Tick();
            await _gameClock.Tick();

            game.Fruits.Should().BeEquivalentTo(new {
                Location = _gameSettings.Fruit,
                Type = FruitType.Cherry
            });
        }
        
        [Fact]
        public async Task FruitShouldDisappearAfterAConfiguredAmountOfTime()
        {
            var fruitLocation = _gameSettings.PacMan.Location.FarAway();
            var game = await PlayGameUntilFruitAppears(fruitLocation);

            await _gameClock.Tick(_now.AddSeconds(_gameSettings.FruitVisibleForSeconds).AddSeconds(1));

            game.Fruits.Should().BeEmpty();
        }

        private async Task<Game> PlayGameUntilFruitAppears(CellLocation fruitLocation){
            _gameSettings.Fruit = fruitLocation;
            _gameSettings.FruitAppearsAfterCoinsEaten.Add(2);

            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.FarAway());

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
            await game.ChangeDirection(Direction.Left);
            await _gameClock.Tick(_now);
            await _gameClock.Tick(_now);

            if (!game.Fruits.Any())
            {
                throw new Exception("Fruit should be visible");
            }

            return game;
        }
        [Fact]
        public async Task PacManCanEatAFruit()
        {
             var fruitLocation = _gameSettings.PacMan.Location.Left.Left.Left;
            var game = await PlayGameUntilFruitAppears(fruitLocation);

            var score = game.Score;
            await _gameClock.Tick();
            
            game.Should().BeEquivalentTo(new {
                Fruits = new Fruit[0],
                Score = score + 100
            });
        }


        [Fact]
        public async Task NotificationsAreRaisedWhenFruitsAreEaten()
        {
            var game = await PlayGameUntilFruitAppears(_gameSettings.PacMan.Location.Left.Left.Left);
            var numberOfNotificationsTriggered = 0;
            game.Subscribe(GameNotification.EatFruit, () => numberOfNotificationsTriggered++);
            
             if (numberOfNotificationsTriggered != 0)
            {
                throw new Exception("No EatFruit notifications should have been triggered yet.");
            }
            
            await _gameClock.Tick();
            
            numberOfNotificationsTriggered.Should().Be(1);
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
            var now = DateTime.UtcNow;
            var gameClock = new TestGameClock();
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left.Left);
            _gameSettings.FruitAppearsAfterCoinsEaten.Add(1);
            
            var game = new Game(gameClock, _gameSettings);
            
            game.StartGame();

            for (var level=0; level<levelNumber-1;level++)
            {
                await game.ChangeDirection(Direction.Left);
                await gameClock.Tick(now);  //Fruit appears
                
                await gameClock.Tick(now);  //Level completed

                if (game.Status != GameStatus.ChangingLevel)
                {
                    throw new Exception($"Game status should be GameStatus.ChangingLevel not {game.Status}");
                }

                WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left.Left);

                await gameClock.Tick(now.AddSeconds(7));  //Screen flashes
                
                WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location);  
            }

            await game.ChangeDirection(Direction.Left);

            await gameClock.Tick(now.AddSeconds(7)); // Eat coin, fruit appears

            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left);

            game.Should().BeEquivalentTo(new {
                Level = levelNumber,
                Fruits = new []{
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
            var now = DateTime.UtcNow;
            var gameClock = new TestGameClock();
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left.Left);
            _gameSettings.Fruit = _gameSettings.PacMan.Location.Left.Above;
            _gameSettings.FruitAppearsAfterCoinsEaten.Add(1);
            
            var game = new Game(gameClock, _gameSettings);
            
            game.StartGame();

            for (var level=0; level<levelNumber-1;level++)
            {
                await game.ChangeDirection(Direction.Left);
                await gameClock.Tick(now);  //Fruit appears
                await gameClock.Tick(now);  //Level completed

                if (game.Status != GameStatus.ChangingLevel)
                {
                    throw new Exception($"Game status should be GameStatus.ChangingLevel not {game.Status}");
                }

                WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left.Left);

                await gameClock.Tick(now.AddSeconds(7));  //Screen flashes
                
                WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location);  
            }

            await game.ChangeDirection(Direction.Left);

            await gameClock.Tick(now.AddSeconds(7)); // Eat coin, fruit appears

            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left);

            await game.ChangeDirection(Direction.Up);

            var score = game.Score;

            await gameClock.Tick(); // Eat coin, fruit appears
            
            game.Should().BeEquivalentTo(new {
                Level = levelNumber,
                Score = score + scoreIncrement
            });
        }
    }
}