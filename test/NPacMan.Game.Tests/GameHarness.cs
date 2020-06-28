using FluentAssertions;
using NPacMan.Game.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NPacMan.Game.Tests
{
    public class GameHarness
    {
        private DateTime _now;
        private bool _createDebugFile;
        private string? _debugFilePath;
        private readonly TestGameClock _gameClock;

        private readonly TestGameSettings _gameSettings;

        public Game Game { get; }

        public int Score => Game.Score;
        public int Lives => Game.Lives;
        public GameStatus Status => Game.Status;

        public Game StartGame() => Game.StartGame();

        public async Task PlayGame()
        {
            StartGame();
            await WaitAndEnterAttractMode();
            await PressStart();
        }

        public GameHarness(TestGameSettings gameSettings, string? debugFilePath = null)
        {
            _gameClock = new TestGameClock();
            _gameSettings = gameSettings;

            Game = new Game(_gameClock, _gameSettings);

            _now = DateTime.UtcNow;

            _createDebugFile = !string.IsNullOrWhiteSpace(debugFilePath);
            _debugFilePath = debugFilePath;

            if (_createDebugFile)
            {
                File.WriteAllText(debugFilePath, $"{DateTime.Now} - {debugFilePath}" + System.Environment.NewLine);
            }
        }

        public async Task EatCoin()
        {
            WriteHeading("EatCoin");

            var currentCoins = Game.Coins.ToList();

            await _gameClock.Tick(_now);

            WriteBoard();

            if (currentCoins.OrderBy(x => x.X).ThenBy(x => x.Y)
                .SequenceEqual(Game.Coins.OrderBy(x => x.X).ThenBy(x => x.Y)))
            {
                WriteAndThrowException("Did not eat coin on tick");
            }
        }

        public async Task EatPill()
        {
            WriteHeading("EatPill");

            var currentPowerPills = Game.PowerPills.ToList();

            await _gameClock.Tick(_now);

            WriteBoard();

            if (currentPowerPills.OrderBy(x => x.X).ThenBy(x => x.Y)
                .SequenceEqual(Game.PowerPills.OrderBy(x => x.X).ThenBy(x => x.Y)))
            {
                WriteAndThrowException("Did not eat power pill on tick");
            }
        }

        public async Task EatFruit()
        {
            WriteHeading("EatFruit");
            
            var numberOfVisibleFruits = Game.Fruits.Length;

            await _gameClock.Tick(_now);

            WriteBoard();

            if (numberOfVisibleFruits == Game.Fruits.Length)
            {
                WriteAndThrowException("Did not fruit");
            }
        }

        public async Task WaitForPauseToComplete()
        {
            WriteHeading("WaitForPauseToComplete");

            _now = _now.AddSeconds(1);
            await _gameClock.Tick(_now);

            WriteBoard();
        }

        public async Task WaitForFruitToDisappear()
        {
            WriteHeading("WaitForFruitToDisappear");

            _now = _now.AddSeconds(_gameSettings.FruitVisibleForSeconds + 1);
            await _gameClock.Tick(_now);

            WriteBoard();
        }

        public async Task WaitToFinishDying()
        {
            WriteHeading("WaitToFinishDying");

            _now = _now.AddSeconds(4);
            await _gameClock.Tick(_now);

            WriteBoard();
        }

        public async Task WaitToRespawn()
        {
            WriteHeading("WaitToRespawn");

            _now = _now.AddSeconds(4);
            await _gameClock.Tick(_now);

            WriteBoard();
        }

        public async Task WaitFourSeconds()
        {
            WriteHeading("WaitFourSeconds");

            _now = _now.AddSeconds(4);
            await _gameClock.Tick(_now);

            WriteBoard();
        }
        public async Task WaitForEndOfLevelFlashingToComplete()
        {
            WriteHeading("WaitForEndOfLevelFlashingToComplete");

            _now = _now.AddSeconds(7);
            await _gameClock.Tick(_now);

            WriteBoard();
        }

        public async Task WaitAndEnterAttractMode()
        {
            WriteHeading("WaitAndEnterAttractMode");

            await WaitFourSeconds();

            WriteBoard();

            EnsureGameStatus(GameStatus.AttractMode);
        }

        internal async Task WaitForFrightenedTimeToComplete()
        {
            WriteHeading("WaitForFrightenedTimeToComplete");

            _now = _now.AddSeconds(_gameSettings.FrightenedTimeInSeconds + 1);

            await _gameClock.Tick(_now);

            WriteBoard();
        }

        internal async Task WaitForScatterToComplete()
        {
            WriteHeading("WaitForScatterToComplete");

            _now = _now.AddSeconds(_gameSettings.InitialScatterTimeInSeconds + 1);

            await _gameClock.Tick(_now);

            WriteBoard();
        }

        internal async Task WaitForChaseToComplete()
        {
            WriteHeading("WaitForChaseToComplete");

            _now = _now.AddSeconds(_gameSettings.ChaseTimeInSeconds + 1);

            await _gameClock.Tick(_now);

            WriteBoard();
        }

        internal async Task WaitFor(TimeSpan delay)
        {
            WriteHeading("WaitFor");

            _now += delay;

            await _gameClock.Tick(_now);

            WriteBoard();
        }
        
        /// <summary>
        /// Trigger a tick,  but we don't expect anything to happen.  For example PacMan
        /// could be in a dying state.
        /// </summary>
        /// <returns></returns>
        public async Task NOP()
        {
            WriteHeading("NOP");

            var pacManLocation = Game.PacMan.Location;
            var ghostLocations = Game.Ghosts.Values.Select(x => x.Location).ToArray();

            var numberOfCoins = Game.Coins.Count;
            var numberOfPowerPills = Game.PowerPills.Count;

            await _gameClock.Tick(_now);

            WriteBoard();

            if (!ghostLocations.SequenceEqual(Game.Ghosts.Values.Select(x => x.Location)))
            {
                WriteAndThrowException("A ghost unexpectedly moved");
            }

            if (Game.PacMan.Location != pacManLocation)
            {
                WriteAndThrowException("PacMan unexpectedly moved");
            }

            if (numberOfCoins != Game.Coins.Count)
            {
                WriteAndThrowException("A coin was unexpectedly eaten");
            }

            if (numberOfPowerPills != Game.PowerPills.Count)
            {
                WriteAndThrowException("A power pill was unexpectedly eaten");
            }
        }

        public async Task Move(string caption = "Move")
        {
            WriteHeading(caption);

            var pacManLocation = Game.PacMan.Location;
            var ghostLocations = Game.Ghosts.Values.Select(x => x.Location).ToArray();

            var numberOfCoins = Game.Coins.Count;
            var numberOfPowerPills = Game.PowerPills.Count;
            var score = Game.Score;

            await _gameClock.Tick(_now);

            WriteBoard();

            if (Game.PacMan.Location == pacManLocation
                && ghostLocations.SequenceEqual(Game.Ghosts.Values.Select(x => x.Location)))
            {
                WriteAndThrowException("Expected PacMan or Ghosts to Move");
            }

            if (numberOfCoins != Game.Coins.Count)
            {
                WriteAndThrowException("A coin was unexpectedly eaten");
            }

            if (numberOfPowerPills != Game.PowerPills.Count)
            {
                WriteAndThrowException("A power pill was unexpectedly eaten");
            }

            if (score != Game.Score)
            {
                WriteAndThrowException("The score unexpectedly increased");
            }
        }

        public async Task GetEatenByGhost(Ghost ghost)
        {
            var actualGhostStatus = Game.Ghosts[ghost.Name].Status;
            if (Game.Ghosts[ghost.Name].Status != GhostStatus.Alive)
            {
                WriteHeading("GetEatenByGhost");
                WriteAndThrowException($"Expected ghost ({ghost.Name}) status to be {GhostStatus.Alive} but was {actualGhostStatus} ");
            }

            await Move("GetEatenByGhost");

            if (Game.Status != GameStatus.Dying && Game.Ghosts[ghost.Name].Location != Game.PacMan.Location)
            {
                WriteAndThrowException($"{ghost.Name} cannot eat PacMan because the ghost is at {Game.Ghosts[ghost.Name].Location} and PacMan is at {Game.PacMan.Location}.");
            }

            EnsureGameStatus(GameStatus.Dying);
        }

        public async Task EatGhost(Ghost ghost)
        {
            WriteHeading("EatGhost");

            var pacManLocation = Game.PacMan.Location;
            var ghostLocations = Game.Ghosts.Values.Select(x => x.Location).ToArray();

            var numberOfCoins = Game.Coins.Count;
            var numberOfPowerPills = Game.PowerPills.Count;
            var score = Game.Score;

            await _gameClock.Tick(_now);

            WriteBoard();

            if (Game.PacMan.Location == pacManLocation
                && ghostLocations.SequenceEqual(Game.Ghosts.Values.Select(x => x.Location)))
            {
                WriteAndThrowException("Expected PacMan or Ghosts to Move");
            }

            var actualGhostStatus = Game.Ghosts[ghost.Name].Status;
            if (Game.Ghosts[ghost.Name].Status != GhostStatus.Score)
            {
                if (Game.Ghosts[ghost.Name].Location != Game.PacMan.Location)
                {
                    WriteAndThrowException($"{ghost.Name} was not eaten by PacMan because the ghost is at {Game.Ghosts[ghost.Name].Location} and PacMan is at {Game.PacMan.Location}.");
                }
                else
                {
                    WriteAndThrowException($"Expected ghost ({ghost.Name}) status to be {GhostStatus.Score} but was {actualGhostStatus} ");
                }
            }
        }

        public async Task EatGhosts(IEnumerable<Ghost> ghosts)
        {
            WriteHeading("EatGhosts");

            var pacManLocation = Game.PacMan.Location;
            var ghostLocations = Game.Ghosts.Values.Select(x => x.Location).ToArray();

            var numberOfCoins = Game.Coins.Count;
            var numberOfPowerPills = Game.PowerPills.Count;
            var score = Game.Score;

            await _gameClock.Tick(_now);

            WriteBoard();

            if (Game.PacMan.Location == pacManLocation
                && ghostLocations.SequenceEqual(Game.Ghosts.Values.Select(x => x.Location)))
            {
                WriteAndThrowException("Expected PacMan or Ghosts to Move");
            }

            foreach (var ghost in ghosts)
            {
                var actualGhostStatus = Game.Ghosts[ghost.Name].Status;
                if (Game.Ghosts[ghost.Name].Status != GhostStatus.Score)
                {
                    WriteAndThrowException($"Expected ghost ({ghost.Name}) status to be {GhostStatus.Score} but was {actualGhostStatus} ");
                }
            }
        }

        public EnsureThatPacMan WeExpectThatPacMan() => new EnsureThatPacMan(Game, Game.PacMan);

        public EnsureThatGhost WeExpectThatGhost(Ghost ghost) => new EnsureThatGhost(Game.Ghosts[ghost.Name]);

        public async Task ChangeDirection(Direction newDirection)
        {
            WriteHeading("ChangeDirection");

            await Game.ChangeDirection(newDirection);

            if (Game.PacMan.Direction != newDirection)
            {
                WriteAndThrowException($"Direction not changed to {newDirection} it's {Game.PacMan.Direction}");
            }
        }



        public async Task PressStart()
        {
            WriteHeading("PressStart");

            await Game.PressStart();
        }

        public void EnsureGameStatus(GameStatus expectedStatus)
        {
            if (Game.Status != expectedStatus)
            {
                WriteAndThrowException($"Game status should be {expectedStatus} not {Game.Status}");
            }
        }

        public async Task AssertSingleNotificationFires(GameNotification gameNotification, Func<Task> action)
        {
            var numberOfNotificationsTriggered = 0;
            Game.Subscribe(gameNotification, () => numberOfNotificationsTriggered++);

            await action();

            numberOfNotificationsTriggered.Should().Be(1);
        }

        internal void Label(string caption)
        {
            WriteHeading(caption);
        }

        private void WriteHeading(string caption)
        {
            if (_createDebugFile)
            {
                var text = caption + System.Environment.NewLine;
                File.AppendAllText(_debugFilePath, text);
            }
        }

        private void WriteAndThrowException(string message)
        {
            if (_createDebugFile)
            {
                var text = "ERROR: " + message + System.Environment.NewLine;
                File.AppendAllText(_debugFilePath, text);
            }

            throw new Exception(message);
        }

        private void WriteBoard()
        {
            if (_createDebugFile)
            {
                File.AppendAllText(_debugFilePath, $"PacMan is at {Game.PacMan.Location} facing {Game.PacMan.Direction}" + System.Environment.NewLine);

                foreach (var ghost in Game.Ghosts.Values)
                {
                    File.AppendAllText(_debugFilePath, $"{ghost.Name} is at {ghost.Location} facing {ghost.Direction} with status {ghost.Status}" + System.Environment.NewLine);
                }

                File.AppendAllText(_debugFilePath,System.Environment.NewLine + "".PadLeft(50, '-') +  System.Environment.NewLine);
            }
        }

        internal void AssertBoard(params string[] rows)
        {
            _gameSettings.AssertBoard(Game, rows);
        }

        internal void EnsureAllGhostsAreEdible()
        {
            if(!Game.Ghosts.Values.All(x => x.Edible))
            {
                throw new Exception("All ghosts should be edible");
            }
        }

        internal async Task WaitForGhostFlash()
        {
            var delay = TimeSpan.FromSeconds(_gameSettings.FrightenedTimeInSeconds - _gameSettings.FrightenedFlashTimeInSeconds);

            WriteHeading("WaitForGhostFlash");

            _now += delay;

            await _gameClock.Tick(_now);

            WriteBoard();
        }
    }
}