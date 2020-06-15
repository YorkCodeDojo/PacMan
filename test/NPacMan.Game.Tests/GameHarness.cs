using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NPacMan.Game.Tests
{
    public class GameHarness
    {
        private readonly Game _game;
        private DateTime _now;
        private readonly TestGameClock _gameClock;

        private readonly IGameSettings _gameSettings;

        public Game Game => _game;

        public GameHarness(IGameSettings gameSettings)
        {
            _gameClock = new TestGameClock();
            _gameSettings = gameSettings;

            _game = new Game(_gameClock, _gameSettings);


            _now = DateTime.UtcNow; ;
        }

        public async Task EatCoin()
        {
            var currentCoins = _game.Coins.ToList();

            await _gameClock.Tick(_now);

            if (currentCoins.OrderBy(x => x.X).ThenBy(x => x.Y)
                .SequenceEqual(_game.Coins.OrderBy(x => x.X).ThenBy(x => x.Y)))
            {
                throw new Exception("Did not eat coin on tick");
            }
        }

        public async Task EatPill()
        {
            var currentPowerPills = _game.PowerPills.ToList();

            await _gameClock.Tick(_now);

            if (currentPowerPills.OrderBy(x => x.X).ThenBy(x => x.Y)
                .SequenceEqual(_game.PowerPills.OrderBy(x => x.X).ThenBy(x => x.Y)))
            {
                throw new Exception("Did not eat power pill on tick");
            }
        }

        public async Task EatGhost()
        {
            await _gameClock.Tick(_now);
        }

        public async Task WaitForPauseToComplete()
        {
            _now = _now.AddSeconds(1);
            await _gameClock.Tick(_now);
        }

        public async Task WaitFourSeconds()
        {
            _now = _now.AddSeconds(4);
            await _gameClock.Tick(_now);
        }

        public async Task WaitAndEnterAttactMode()
        {
            await WaitFourSeconds();

            EnsureGameStatus(GameStatus.AttractMode);
        }

        public async Task Move()
        {
            var pacManLocation = Game.PacMan.Location;
            var ghostLocations = Game.Ghosts.Values.Select(x => x.Location).ToArray();

            await _gameClock.Tick(_now);

            if (_game.PacMan.Location == pacManLocation
                && ghostLocations.SequenceEqual(Game.Ghosts.Values.Select(x => x.Location)))
            {
                throw new Exception("Expected PacMan or Ghosts to Move");
            }
        }

        public async Task GetEatenByGhost()
        {
            await Move();
            EnsureGameStatus(GameStatus.Dying);
        }

        public async Task ChangeDirection(Direction newDirection)
        {
            await _game.ChangeDirection(newDirection);

            if (_game.PacMan.Direction != newDirection)
            {
                throw new Exception($"Direction not changed to {newDirection} it's {_game.PacMan.Direction}");
            }
        }

        public async Task PressStart()
        {
            await _game.PressStart();
        }

        public void EnsureGameStatus(GameStatus expectedStatus)
        {
            if (_game.Status != expectedStatus)
            {
                throw new Exception($"Game status should be {expectedStatus} not {_game.Status}");
            }
        }
    }
}