using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using System.Linq;
using NPacMan.Game.Tests.Helpers;
using System.Threading.Tasks;
using System;

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
    }
}