using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using System;
using System.Threading.Tasks;
using NPacMan.Game.Tests.GhostStrategiesForTests;

namespace NPacMan.Game.Tests.GameTests
{
    public class PowerPillTests
    {
        private readonly TestGameSettings _gameSettings;
        private readonly TestGameClock _gameClock;

        public PowerPillTests()
        {
            _gameSettings = new TestGameSettings();
            _gameClock = new TestGameClock();
        }

        [Fact]
        public async Task IncrementsScoreBy50WhenPowerPillCollected()
        {
            var game = new Game(_gameClock, _gameSettings);
            _gameSettings.PowerPills.Add(game.PacMan.Location.Below);

            game.StartGame(); 
            game.ChangeDirection(Direction.Down);
            await _gameClock.Tick();

            game.Score.Should().Be(50);
        }

             [Fact]
        public async Task CannotCollectTheSamePowerPillTwice()
        {
            var game = new Game(_gameClock, _gameSettings);
            _gameSettings.PowerPills.Add(game.PacMan.Location.Below);
            game.StartGame();

            game.ChangeDirection(Direction.Down);
            await _gameClock.Tick();

            if (game.Score != 50)
                throw new Exception($"Score should be 50 not {game.Score}");

            game.ChangeDirection(Direction.Up);
            await _gameClock.Tick();

            if (game.Score != 50)
                throw new Exception($"Score should still be 50 not {game.Score}");

            game.ChangeDirection(Direction.Down);
            await _gameClock.Tick();

            game.Score.Should().Be(50);
        }

        [Fact]
        public async Task WhenPacManEatsAPowerPillTheGameNotificationShouldFire()
        {
            var numberOfNotificationsTriggered = 0;

            var game = new Game(_gameClock, _gameSettings);
            _gameSettings.PowerPills.Add(game.PacMan.Location.Below);
            game.Subscribe(GameNotification.EatPowerPill, () => numberOfNotificationsTriggered++);
            game.StartGame(); 

            game.ChangeDirection(Direction.Down);

            await _gameClock.Tick();

            numberOfNotificationsTriggered.Should().Be(1);
        }
    
    }
}