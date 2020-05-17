using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using System;
using System.Threading.Tasks;
using NPacMan.Game.Tests.GhostStrategiesForTests;
using NPacMan.Game.Tests.Helpers;

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
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Below);
            var game = new Game(_gameClock, _gameSettings);

            game.StartGame(); 
            game.ChangeDirection(Direction.Down);
            await _gameClock.Tick();

            game.Score.Should().Be(50);
        }

        [Fact]
        public async Task GameStateStaysAlivewhenPillCollected()
        {
            var game = new Game(_gameClock, _gameSettings);
            _gameSettings.PowerPills.Add(game.PacMan.Location.Below);

            game.StartGame(); 
            game.ChangeDirection(Direction.Down);
            await _gameClock.Tick();

            game.Status.Should().Be(GameStatus.Alive);
        }

        [Fact]
        public async Task CannotCollectTheSamePowerPillTwice()
        {
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Below);
            var game = new Game(_gameClock, _gameSettings);
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
        public async Task GameContainsAllPowerPills()
        {
            var gameBoard = new TestGameSettings();
            gameBoard.PowerPills.Add((1, 1));
            gameBoard.PowerPills.Add((1, 2));
            gameBoard.PowerPills.Add((2, 2));

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, gameBoard);
            game.StartGame();
            await gameClock.Tick();

            game.PowerPills.Should().BeEquivalentTo(
                new CellLocation(1, 1),
                new CellLocation(1, 2),
                new CellLocation(2, 2)
            );
        }

        [Fact]
        public async Task PacManDoesNotEatPowerPillAndScoreStaysTheSameWhenCollidesWithGhost()
        {
            var x = _gameSettings.PacMan.Location.X + 1;
            var y = _gameSettings.PacMan.Location.Y;

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", _gameSettings.PacMan.Location.Right, Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame(); 
            var score = game.Score;

            game.ChangeDirection(Direction.Right);
            await _gameClock.Tick();

            using var _ = new AssertionScope();
            game.PowerPills.Should().ContainEquivalentOf(new {
                X = x,
                Y = y
            });
            game.Score.Should().Be(score);
        }


        [Fact]
        public async Task AllGhostsShouldChangetoEdibleWhenPacManEatsPowerPill()
        {
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", _gameSettings.PacMan.Location.Right.Right, Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));
            _gameSettings.Ghosts.Add(new Ghost("Ghost2", _gameSettings.PacMan.Location.Right.Right, Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));
            _gameSettings.Ghosts.Add(new Ghost("Ghost3", _gameSettings.PacMan.Location.Right.Right, Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);

             var game = new Game(_gameClock, _gameSettings);
            game.StartGame(); 

            game.ChangeDirection(Direction.Right);
            await _gameClock.Tick();

            game.Ghosts.Values.Should().AllBeEquivalentTo(new {
                Edible = true
            });
        }

        [Fact]
        public async Task WhenPacManEatsAPowerPillTheGameNotificationShouldFire()
        {
            var numberOfNotificationsTriggered = 0;

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Below);
            var game = new Game(_gameClock, _gameSettings);
            game.Subscribe(GameNotification.EatPowerPill, () => numberOfNotificationsTriggered++);
            game.StartGame(); 

            game.ChangeDirection(Direction.Down);

            await _gameClock.Tick();

            numberOfNotificationsTriggered.Should().Be(1);
        }
    
        private EnsureThatGhost WeExpectThat(Ghost ghost) => new EnsureThatGhost(ghost);
    
    }
}