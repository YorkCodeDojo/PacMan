using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Automatonymous;

namespace NPacMan.Game
{
    public class Game : IGameActions
    {
        public int Score => _gameState.Score;

        private readonly IGameClock _gameClock;
        private readonly IGameSettings _settings;
        private List<CellLocation> _collectedCoins;
        private Dictionary<string, Ghost> _ghosts;

        private readonly GameNotifications _gameNotifications = new GameNotifications();

        private readonly GameState _gameState;

        private readonly GameStateMachine _gameStateMachine;

        public Game(IGameClock gameClock, IGameSettings settings)
        {
            _gameClock = gameClock;
            _settings = settings;
            PacMan = settings.PacMan;
            _collectedCoins = new List<CellLocation>();
            _ghosts = settings.Ghosts.ToDictionary(x => x.Name, x => x);
            _gameStateMachine = new GameStateMachine(this, settings, _gameNotifications);
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

        public PacMan PacMan { get; private set; }

        public IReadOnlyCollection<CellLocation> Coins
            => _settings.Coins.Except(_collectedCoins).ToList().AsReadOnly();

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

        public IReadOnlyDictionary<string, Ghost> Ghosts
            => _gameState.GhostsVisible ? _ghosts : (IReadOnlyDictionary<string, Ghost>)ImmutableDictionary<string, Ghost>.Empty;

        public GameStatus Status => _gameState.Status switch
        {
            nameof(GameStateMachine.Initial) => GameStatus.Initial,
            nameof(GameStateMachine.Alive) => GameStatus.Alive,
            nameof(GameStateMachine.Scatter) => GameStatus.Alive,
            nameof(GameStateMachine.Dying) => GameStatus.Dying,
            nameof(GameStateMachine.Respawning) => GameStatus.Respawning,
            nameof(GameStateMachine.Dead) => GameStatus.Dead,
            _ => throw new NotImplementedException($"No map for status '{_gameState.Status}")
        };

        public void ChangeDirection(Direction direction)
        {
            var nextSpace = PacMan.Location + direction;
            if(!Walls.Contains(nextSpace))
            {
                PacMan = PacMan.WithNewDirection(direction);
            }
        }

        private async Task Tick(DateTime now)
        {
            await _gameStateMachine.RaiseEvent(_gameState, _gameStateMachine.Tick, new Tick(now));
        }

        private bool HasDied()
        {
            return Ghosts.Values.Any(ghost => ghost.Location.X == PacMan.Location.X && ghost.Location.Y == PacMan.Location.Y);
        }

        private void ApplyToGhosts(Func<Ghost, Ghost> action)
        {
            var newPositionOfGhosts = new Dictionary<string, Ghost>();
            foreach (var ghost in _ghosts.Values)
            {
                newPositionOfGhosts[ghost.Name] = action(ghost);
            }

            _ghosts = newPositionOfGhosts;
        }


        void IGameActions.ShowGhosts(GameState gameState)
        {
            gameState.GhostsVisible = true;
        }


        Task IGameActions.MoveGhosts(DateTime now)
        {
            return MoveGhosts(now);
        }

        void IGameActions.HideGhosts(GameState gameState)
        {
            gameState.GhostsVisible = false;
        }

        void IGameActions.ScatterGhosts()
        {
            ApplyToGhosts(ghost => ghost.Scatter());
        }

        void IGameActions.GhostToChase()
        {
            ApplyToGhosts(ghost => ghost.Chase());
        }

        void IGameActions.MoveGhostsHome()
        {
            ApplyToGhosts(ghost => ghost.SetToHome());
        }

        void IGameActions.MovePacManHome()
        {
            PacMan = PacMan.SetToHome();
        }

        async Task IGameActions.MovePacMan(DateTime now)
        {
            var newPacMan = PacMan.Move();

            if (_settings.Portals.TryGetValue(newPacMan.Location, out var portal))
            {
                newPacMan = PacMan.WithNewX(portal.X).WithNewY(portal.Y);
                newPacMan = newPacMan.Move();
            }

            if (!_settings.Walls.Contains(newPacMan.Location))
            {
                PacMan = newPacMan;
            }

            if (HasDied())
            {
                await _gameStateMachine.RaiseEvent(_gameState, _gameStateMachine.PacManCaughtByGhost, new Tick(now));
            }
            else if (Coins.Contains(newPacMan.Location))
            {
                var newCollectedCoins = new List<CellLocation>(_collectedCoins)
                    {
                        (newPacMan.Location)
                    };
                _collectedCoins = newCollectedCoins;

                await _gameStateMachine.RaiseEvent(_gameState, _gameStateMachine.CoinEaten);
            }
        }

        private async Task MoveGhosts(DateTime now)
        {
            ApplyToGhosts(ghost => ghost.Move(this));
            if (HasDied())
            {
                await _gameStateMachine.RaiseEvent(_gameState, _gameStateMachine.PacManCaughtByGhost, new Tick(now));
            }
        }
    }
}
