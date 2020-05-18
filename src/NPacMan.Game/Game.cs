using Automatonymous;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace NPacMan.Game
{
    public class Game
    {

        private readonly IGameClock _gameClock;

        private readonly IGameSettings _settings;

        private readonly GameNotifications _gameNotifications = new GameNotifications();

        private readonly GameState _gameState;

        private readonly GameStateMachine _gameStateMachine;

        public Game(IGameClock gameClock, IGameSettings settings)
        {
            _gameClock = gameClock;
            _settings = settings;
            _gameStateMachine = new GameStateMachine(settings, _gameNotifications, this);
            _gameState = new GameState(settings);
        }

        public Game StartGame()
        {
            _gameClock.Subscribe(Tick);

            return this;
        }

        public Game Subscribe(GameNotification gameNotification, Action action)
        {
            _gameNotifications.Subscribe(gameNotification, action);

            return this;
        }

        public static Game Create()
        {
            var filename = "board.txt";
            var gameSettings = GameSettingsLoader.LoadFromFile(filename);

            return new Game(new GameClock(), gameSettings);
        }

        public IReadOnlyCollection<CellLocation> Coins
            => _gameState.RemainingCoins.AsReadOnly();

        public IReadOnlyCollection<CellLocation> PowerPills
            => _gameState.RemainingPowerPills.AsReadOnly();

        public IReadOnlyCollection<CellLocation> Walls
            => _settings.Walls;

        public IReadOnlyCollection<CellLocation> Doors
            => _settings.Doors;

        public int Width
            => _settings.Width;

        public int Height
            => _settings.Height;

        public int Lives
            => _gameState.Lives;

        public int Score => _gameState.Score;
        public PacMan PacMan => _gameState.PacMan;

        public IReadOnlyDictionary<string, Ghost> Ghosts
            => _gameState.GhostsVisible ? _gameState.Ghosts : (IReadOnlyDictionary<string, Ghost>)ImmutableDictionary<string, Ghost>.Empty;

        public GameStatus Status => _gameState.Status switch
        {
            nameof(GameStateMachine.Initial) => GameStatus.Initial,
            nameof(GameStateMachine.GhostChase) => GameStatus.Alive,
            nameof(GameStateMachine.Scatter) => GameStatus.Alive,
            nameof(GameStateMachine.Frightened) => GameStatus.Alive,
            nameof(GameStateMachine.Dying) => GameStatus.Dying,
            nameof(GameStateMachine.Respawning) => GameStatus.Respawning,
            nameof(GameStateMachine.Dead) => GameStatus.Dead,
            _ => throw new NotImplementedException($"No map for status '{_gameState.Status}'")
        };

        private async Task Tick(DateTime now)
        {
            await _gameStateMachine.RaiseEvent(_gameState, _gameStateMachine.Tick, new Tick(now));
        }
        public void ChangeDirection(Direction direction)
        {
            var nextSpace = _gameState.PacMan.Location + direction;
            if (!Walls.Contains(nextSpace))
            {
                _gameState.PacMan = _gameState.PacMan.WithNewDirection(direction);
            }
        }
    }
}
