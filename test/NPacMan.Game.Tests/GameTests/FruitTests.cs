using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using System.Linq;
using NPacMan.Game.Tests.Helpers;
using System.Threading.Tasks;

namespace NPacMan.Game.Tests.GameTests
{
    public class FruitTests
    {
        private readonly TestGameSettings _gameSettings;
        private readonly TestGameClock _gameClock;

        public FruitTests()
        {
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
    }
}