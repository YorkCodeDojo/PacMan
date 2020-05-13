using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using Automatonymous;

namespace NPacMan.Game
{
    public class GameState
    {
        public string Status { get; set; } = null!;

        public DateTime? TimeToChangeState { get; set; }

        public int Lives { get; set; }

        public bool GhostsVisible { get; set; }

        public int Score { get; set; }
    }


    class Tick
    {
        public DateTime Now { get; set; }
    }

    public partial class Game
    {
        public int Score => _gameState.Score;

        private readonly IGameSettings _settings;
        private List<CellLocation> _collectedCoins;
        private Dictionary<string, Ghost> _ghosts;

        private GameState _gameState;

        private GameNotifications _gameNotifications;

        private GameStateMachine _gameStateMachine;
        public Game(IGameClock gameClock, IGameSettings settings, GameNotifications? gameNotifications = null)
        {
            gameClock.Subscribe(Tick);
            _settings = settings;
            PacMan = settings.PacMan;
            _collectedCoins = new List<CellLocation>();
            _ghosts = settings.Ghosts.ToDictionary(x => x.Name, x => x);
            _gameStateMachine = new GameStateMachine(this);
            _gameState = new GameState
            {
                Lives = settings.InitialLives,
                Status = settings.InitialGameStatus,
                Score = 0,
                GhostsVisible = true,
                TimeToChangeState = null
            };
            _gameNotifications = gameNotifications ?? new GameNotifications();
        }

        public static Game Create(GameNotifications gameNotifications)
        {
            var filename = "board.txt";
            var gameSettings = GameSettingsLoader.LoadFromFile(filename);

            return new Game(new GameClock(), gameSettings, gameNotifications);
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

        public string Status => _gameState.Status;

        public void ChangeDirection(Direction direction)
        {
            PacMan = PacMan.WithNewDirection(direction);
        }

        private void Tick(DateTime now)
        {
            _gameStateMachine.RaiseEvent(_gameState, _gameStateMachine.Tick, new Tick { Now = now });
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

     
        private void ScatterGhosts()
        {
            ApplyToGhosts(ghost => ghost.Scatter());
        }

        private void GhostToChase()
        {
            ApplyToGhosts(ghost => ghost.Chase());
        }

        private void HideGhosts()
        {
            _gameState.GhostsVisible = false;
        }

        private void MoveGhostsHome()
        {
            ApplyToGhosts(ghost => ghost.SetToHome());
        }

        private void MovePacManHome()
        {
            PacMan = PacMan.SetToHome();
        }
        private void MovePacMan(DateTime now)
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
                _gameStateMachine.RaiseEvent(_gameState, _gameStateMachine.PacManCaughtByGhost, new Tick { Now = now });
            }
            else if (Coins.Contains(newPacMan.Location))
            {
                var newCollectedCoins = new List<CellLocation>(_collectedCoins)
                    {
                        (newPacMan.Location)
                    };
                _collectedCoins = newCollectedCoins;

                _gameStateMachine.RaiseEvent(_gameState, _gameStateMachine.CoinEaten);
            }
        }

        private void MoveGhosts(DateTime now)
        {
            ApplyToGhosts(ghost => ghost.Move(this));
            if (HasDied())
            {
                _gameStateMachine.RaiseEvent(_gameState, _gameStateMachine.PacManCaughtByGhost, new Tick { Now = now });
            }
        }

        private void ShowGhosts()
        {
            _gameState.GhostsVisible = true;
        }
    }
}
