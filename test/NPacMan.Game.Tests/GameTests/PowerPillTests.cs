using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using System;
using System.Threading.Tasks;
using NPacMan.Game.Tests.GhostStrategiesForTests;
using System.Collections.Generic;
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
            await game.ChangeDirection(Direction.Down);
            await _gameClock.Tick();

            game.Score.Should().Be(50);
        }

        [Fact]
        public async Task GameStateStaysAlivewhenPillCollected()
        {
            var game = new Game(_gameClock, _gameSettings);
            _gameSettings.PowerPills.Add(game.PacMan.Location.Below);

            game.StartGame();
            await game.ChangeDirection(Direction.Down);
            await _gameClock.Tick();

            game.Status.Should().Be(GameStatus.Alive);
        }

        [Fact]
        public async Task CannotCollectTheSamePowerPillTwice()
        {
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.FarAway());
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Below);
            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Down);
            await _gameClock.Tick();

            if (game.Score != 50)
                throw new Exception($"Score should be 50 not {game.Score}");

            await game.ChangeDirection(Direction.Up);
            await _gameClock.Tick();

            if (game.Score != 50)
                throw new Exception($"Score should still be 50 not {game.Score}");

            await game.ChangeDirection(Direction.Down);
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
            var pillAndGhostLocation = _gameSettings.PacMan.Location + Direction.Right;

            var ghost = GhostBuilder.New()
                .WithLocation(pillAndGhostLocation)
                .Create();

            _gameSettings.Ghosts.Add(ghost);
            _gameSettings.PowerPills.Add(pillAndGhostLocation);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
            var score = game.Score;

            await game.ChangeDirection(Direction.Right);
            await _gameClock.Tick();

            using var _ = new AssertionScope();
            game.PowerPills.Should().ContainEquivalentOf(pillAndGhostLocation);
            game.Score.Should().Be(score);
        }


        [Fact]
        public async Task AllGhostsShouldChangetoEdibleWhenPacManEatsPowerPill()
        {
            var ghosts = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .CreateMany(3);
            _gameSettings.Ghosts.AddRange(ghosts);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Right);
            await _gameClock.Tick();

            game.Ghosts.Values.Should().AllBeEquivalentTo(new
            {
                Edible = true
            });
        }

        [Fact]
        public async Task AllGhostsShouldChangeDirectionWhenPacManEatsPowerPill()
        {
            var ghost1 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .WithDirection(Direction.Up)
                .Create();
            var ghost2 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .WithDirection(Direction.Down)
                .Create();
            var ghost3 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .WithDirection(Direction.Left)
                .Create();
            var ghost4 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .WithDirection(Direction.Right)
                .Create();
            _gameSettings.Ghosts.AddRange(new[] { ghost1, ghost2, ghost3, ghost4 });

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.FarAway());
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Right);
            await _gameClock.Tick();

            game.Ghosts.Should().BeEquivalentTo(new Dictionary<string, object>
            {
                [ghost1.Name] = new
                {
                    Direction = ghost1.Direction.Opposite()
                },
                [ghost2.Name] = new
                {
                    Direction = ghost2.Direction.Opposite()
                },
                [ghost3.Name] = new
                {
                    Direction = ghost3.Direction.Opposite()
                },
                [ghost4.Name] = new
                {
                    Direction = ghost4.Direction.Opposite()
                },
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

            await game.ChangeDirection(Direction.Down);

            await _gameClock.Tick();

            numberOfNotificationsTriggered.Should().Be(1);
        }
    }
}