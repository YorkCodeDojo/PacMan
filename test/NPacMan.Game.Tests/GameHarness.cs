using FluentAssertions;
using NPacMan.Game.Tests.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NPacMan.Game.Tests
{
    public class GameHarness
    {
        private DateTime _now;
        private readonly TestGameClock _gameClock;

        private readonly IGameSettings _gameSettings;

        public Game Game { get; }

        public int Score => Game.Score;
        public int Lives => Game.Lives;
        public GameStatus Status => Game.Status;

        public Game StartGame() => Game.StartGame();

        public GameHarness(IGameSettings gameSettings)
        {
            _gameClock = new TestGameClock();
            _gameSettings = gameSettings;

            Game = new Game(_gameClock, _gameSettings);

            _now = DateTime.UtcNow;
        }

        public async Task EatCoin()
        {
            var currentCoins = Game.Coins.ToList();

            await _gameClock.Tick(_now);

            if (currentCoins.OrderBy(x => x.X).ThenBy(x => x.Y)
                .SequenceEqual(Game.Coins.OrderBy(x => x.X).ThenBy(x => x.Y)))
            {
                throw new Exception("Did not eat coin on tick");
            }
        }

        public async Task EatPill()
        {
            var currentPowerPills = Game.PowerPills.ToList();

            await _gameClock.Tick(_now);

            if (currentPowerPills.OrderBy(x => x.X).ThenBy(x => x.Y)
                .SequenceEqual(Game.PowerPills.OrderBy(x => x.X).ThenBy(x => x.Y)))
            {
                throw new Exception("Did not eat power pill on tick");
            }
        }

        public async Task EatFruit()
        {
            var numberOfVisibleFruits = Game.Fruits.Length;

            await _gameClock.Tick(_now);

            if (numberOfVisibleFruits == Game.Fruits.Length)
            {
                throw new Exception("Did not fruit");
            }
        }

        public async Task WaitForPauseToComplete()
        {
            _now = _now.AddSeconds(1);
            await _gameClock.Tick(_now);
        }

        public async Task WaitForFruitToDisappear()
        {
            _now = _now.AddSeconds(_gameSettings.FruitVisibleForSeconds + 1);
            await _gameClock.Tick(_now);
        }

        public async Task WaitToFinishDying()
        {
            _now = _now.AddSeconds(4);
            await _gameClock.Tick(_now);
        }

        public async Task WaitToRespawn()
        {
            _now = _now.AddSeconds(4);
            await _gameClock.Tick(_now);
        }

        public async Task WaitFourSeconds()
        {
            _now = _now.AddSeconds(4);
            await _gameClock.Tick(_now);
        }
        public async Task WaitForEndOfLevelFlashingToComplete()
        {
            _now = _now.AddSeconds(7);
            await _gameClock.Tick(_now);
        }

        public async Task WaitAndEnterAttractMode()
        {
            await WaitFourSeconds();

            EnsureGameStatus(GameStatus.AttractMode);
        }

        /// <summary>
        /// Trigger a tick,  but we don't expect anything to happen.  For example PacMan
        /// could be in a dying state.
        /// </summary>
        /// <returns></returns>
        public async Task NOP()
        {
            var pacManLocation = Game.PacMan.Location;
            var ghostLocations = Game.Ghosts.Values.Select(x => x.Location).ToArray();

            var numberOfCoins = Game.Coins.Count;
            var numberOfPowerPills = Game.PowerPills.Count;

            await _gameClock.Tick(_now);

            if (!ghostLocations.SequenceEqual(Game.Ghosts.Values.Select(x => x.Location)))
            {
                throw new Exception("A ghost unexpectedly moved");
            }

            if (Game.PacMan.Location != pacManLocation)
            {
                throw new Exception("PacMan unexpectedly moved");
            }

            if (numberOfCoins != Game.Coins.Count)
            {
                throw new Exception("A coin was unexpectedly eaten");
            }

            if (numberOfPowerPills != Game.PowerPills.Count)
            {
                throw new Exception("A power pill was unexpectedly eaten");
            }
        }

        public async Task Move()
        {
            var pacManLocation = Game.PacMan.Location;
            var ghostLocations = Game.Ghosts.Values.Select(x => x.Location).ToArray();

            var numberOfCoins = Game.Coins.Count;
            var numberOfPowerPills = Game.PowerPills.Count;

            await _gameClock.Tick(_now);

            if (Game.PacMan.Location == pacManLocation
                && ghostLocations.SequenceEqual(Game.Ghosts.Values.Select(x => x.Location)))
            {
                throw new Exception("Expected PacMan or Ghosts to Move");
            }

            if (numberOfCoins != Game.Coins.Count)
            {
                throw new Exception("A coin was unexpectedly eaten");
            }

            if (numberOfPowerPills != Game.PowerPills.Count)
            {
                throw new Exception("A power pill was unexpectedly eaten");
            }
        }

        public async Task GetEatenByGhost(Ghost ghost)
        {
            var actualGhostStatus = Game.Ghosts[ghost.Name].Status;
            if (Game.Ghosts[ghost.Name].Status != GhostStatus.Alive)
            {
                throw new Exception($"Expected ghost ({ghost.Name}) status to be {GhostStatus.Alive} but was {actualGhostStatus} ");
            }

            await Move();
            EnsureGameStatus(GameStatus.Dying);
        }

        public async Task EatGhost(Ghost ghost)
        {
            await Move();

            var actualGhostStatus = Game.Ghosts[ghost.Name].Status;
            if (Game.Ghosts[ghost.Name].Status != GhostStatus.Score)
            {
                throw new Exception($"Expected ghost ({ghost.Name}) status to be {GhostStatus.Score} but was {actualGhostStatus} ");
            }
        }

        public EnsureThatPacMan WeExpectThatPacMan() => new EnsureThatPacMan(Game.PacMan);

        public EnsureThatGhost WeExpectThatGhost(Ghost ghost) => new EnsureThatGhost(Game.Ghosts[ghost.Name]);

        public async Task ChangeDirection(Direction newDirection)
        {
            await Game.ChangeDirection(newDirection);

            if (Game.PacMan.Direction != newDirection)
            {
                throw new Exception($"Direction not changed to {newDirection} it's {Game.PacMan.Direction}");
            }
        }

        public async Task PressStart()
        {
            await Game.PressStart();
        }

        public void EnsureGameStatus(GameStatus expectedStatus)
        {
            if (Game.Status != expectedStatus)
            {
                throw new Exception($"Game status should be {expectedStatus} not {Game.Status}");
            }
        }

        public async Task AssertSingleNotificationFires(GameNotification gameNotification, Func<Task> action)
        {
            var numberOfNotificationsTriggered = 0;
            Game.Subscribe(gameNotification, () => numberOfNotificationsTriggered++);

            await action();

            //if (numberOfNotificationsTriggered != 1)
            //{
            //    throw new Exception($"A single {gameNotification} notifications should have been triggered but {numberOfNotificationsTriggered} were.");
            //}

            numberOfNotificationsTriggered.Should().Be(1);
        }

        internal async Task WaitForFrightenedTimeToComplete()
        {
            _now = _now.AddSeconds(_gameSettings.FrightenedTimeInSeconds + 1);

            await _gameClock.Tick(_now);
        }

        internal void Label(string caption)
        {
        }

        internal async Task WaitFor(TimeSpan delay)
        {
            _now+= delay;

            await _gameClock.Tick(_now);
        }
    }
}