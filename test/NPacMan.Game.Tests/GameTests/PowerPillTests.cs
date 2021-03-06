using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using NPacMan.Game.Tests.Helpers;
using NPacMan.Game.Tests.GhostStrategiesForTests;

namespace NPacMan.Game.Tests.GameTests
{
    public class PowerPillTests
    {
        private readonly TestGameSettings _gameSettings;

        public PowerPillTests()
        {
            _gameSettings = new TestGameSettings();
        }

        [Fact]
        public async Task IncrementsScoreBy50WhenPowerPillCollected()
        {
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Below);
            var gameHarness = new GameHarness(_gameSettings);

            await gameHarness.PlayGame();
            await gameHarness.ChangeDirection(Direction.Down);
            await gameHarness.EatPill();

            gameHarness.Score.Should().Be(50);
        }

        [Fact]
        public async Task GameStateStaysAlivewhenPillCollected()
        {
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Below);
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.FarAway());
            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Down);
            await gameHarness.EatPill();

            gameHarness.Status.Should().Be(GameStatus.Alive);
        }

        [Fact]
        public async Task CannotCollectTheSamePowerPillTwice()
        {
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.FarAway());
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Below);
            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Down);
            await gameHarness.EatPill();

            await gameHarness.ChangeDirection(Direction.Up);
            await gameHarness.Move();

            await gameHarness.ChangeDirection(Direction.Down);
            await gameHarness.Move();

            gameHarness.Score.Should().Be(50);
        }

        [Fact]
        public async Task GameContainsAllPowerPills()
        {
            var powerPill1 = _gameSettings.PacMan.Location.FarAway().Left;
            var powerPill2 = _gameSettings.PacMan.Location.FarAway();
            var powerPill3 = _gameSettings.PacMan.Location.FarAway().Right;

            _gameSettings.PowerPills.Add(powerPill1);
            _gameSettings.PowerPills.Add(powerPill2);
            _gameSettings.PowerPills.Add(powerPill3);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.Move();

            gameHarness.Game.PowerPills.Should().BeEquivalentTo(
                powerPill1,
                powerPill2,
                powerPill3
            );
        }

        [Fact]
        public async Task PacManDoesNotEatPowerPillAndScoreStaysTheSameWhenCollidesWithGhost()
        {
            var pillAndGhostLocation = _gameSettings.PacMan.Location + Direction.Right;

            var ghost = GhostBuilder.New()
                .WithLocation(pillAndGhostLocation)
                .WithScatterStrategy(new StandingStillGhostStrategy())
                .Create();

            _gameSettings.Ghosts.Add(ghost);
            _gameSettings.PowerPills.Add(pillAndGhostLocation);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            var score = gameHarness.Score;

            await gameHarness.ChangeDirection(Direction.Right);
            await gameHarness.GetEatenByGhost(ghost);

            using var _ = new AssertionScope();
            gameHarness.Game.PowerPills.Should().ContainEquivalentOf(pillAndGhostLocation);
            gameHarness.Score.Should().Be(score);
        }

        [Fact]
        public async Task AllGhostsShouldChangetoEdibleWhenPacManEatsPowerPill()
        {
            var ghosts = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .CreateMany(3);
            _gameSettings.Ghosts.AddRange(ghosts);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Right);
            await gameHarness.EatPill();

            gameHarness.Game.Ghosts.Values.Should().AllBeEquivalentTo(new
            {
                Edible = true
            });
        }

        [Fact]
        public async Task AllGhostsShouldChangeDirectionWhenPacManEatsPowerPill()
        {
            var ghost1 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .WithScatterStrategyUp()
                .Create();
            var ghost2 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .WithScatterStrategyDown()
                .Create();
            var ghost3 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .WithScatterStrategyLeft()
                .Create();
            var ghost4 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .WithScatterStrategyRight()
                .Create();
            _gameSettings.Ghosts.AddRange(new[] { ghost1, ghost2, ghost3, ghost4 });

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.FarAway());
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Right);
            await gameHarness.EatPill();

            gameHarness.Game.Ghosts.Should().BeEquivalentTo(new Dictionary<string, object>
            {
                [ghost1.Name] = new
                {
                    Direction = Direction.Down
                },
                [ghost2.Name] = new
                {
                    Direction = Direction.Up
                },
                [ghost3.Name] = new
                {
                    Direction = Direction.Right
                },
                [ghost4.Name] = new
                {
                    Direction = Direction.Left
                },
            });
        }

        [Fact]
        public async Task WhenPacManEatsAPowerPillTheGameNotificationShouldFire()
        {
            var numberOfNotificationsTriggered = 0;

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Below);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.Game.Subscribe(GameNotification.EatPowerPill, () => numberOfNotificationsTriggered++);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Down);

            await gameHarness.EatPill();

            numberOfNotificationsTriggered.Should().Be(1);
        }
    }
}