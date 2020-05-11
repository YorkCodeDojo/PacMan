using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

namespace NPacMan.Game
{
    public class GameState
    {
        public GameStatus Status { get; set; }
        
        public DateTime? TimeToChangeState { get; set;}

        public int Lives { get; set; }
    }

    public class Game
    {
        public int Score { get; private set; }

        private readonly IGameSettings _settings;
        private List<CellLocation> _collectedCoins;
        private Dictionary<string, Ghost> _ghosts;

        private ISoundSet _soundSet;

        private GameState _gameState;

        public Game(IGameClock gameClock, IGameSettings settings, ISoundSet soundSet)
        {
            gameClock.Subscribe(Tick);
            _settings = settings;
            PacMan = settings.PacMan;
            _collectedCoins = new List<CellLocation>();
            _ghosts = settings.Ghosts.ToDictionary(x => x.Name, x => x);
            _soundSet = soundSet;
            _gameState = new GameState{
                Lives = settings.InitialLives,
                Status = settings.InitialGameStatus
            };

            // Play the beginning sound
            _soundSet.Beginning();
        }

        public Game(IGameClock gameClock, IGameSettings settings) 
            : this(gameClock, settings, new NullSoundSet())
        {
        }

        public static Game Create(ISoundSet soundSet)
        {
            var filename = "board.txt";
            var gameSettings = GameSettingsLoader.LoadFromFile(filename);

            return new Game(new GameClock(), gameSettings, soundSet);
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
            => _gameState.Status == GameStatus.Respawning ? (IReadOnlyDictionary<string, Ghost>)ImmutableDictionary<string, Ghost>.Empty : _ghosts;

        public GameStatus Status => _gameState.Status;

        public void ChangeDirection(Direction direction)
        {
            PacMan = PacMan.WithNewDirection(direction);
        }

        private void Tick(DateTime now)
        {
            var oldState = _gameState.Status;

            UpdateStates(now);
            var newPacMan = PacMan.Transition(now);

            if (_settings.Portals.TryGetValue(newPacMan.Location, out var portal))
            {
                newPacMan = PacMan.WithNewX(portal.X).WithNewY(portal.Y);
                UpdateStates(now);
                newPacMan = newPacMan.Transition(now);
            }

            if (oldState == GameStatus.Alive)
            {
                ApplyToGhosts(ghost => ghost.Move(this));
            }

            if (_gameState.Status != GameStatus.Alive)
            {
                return;
            }

            if (HasDied())
            {
                _gameState.TimeToChangeState = now.AddSeconds(4);
                _gameState.Lives -= 1;
                _gameState.Status  = GameStatus.Dying;
                return;
            }

            if (oldState == GameStatus.Alive && !_settings.Walls.Contains(newPacMan.Location))
            {
                PacMan = newPacMan;
            }

            if (HasDied())
            {
                _gameState.TimeToChangeState = now.AddSeconds(4);
                _gameState.Lives -= 1;
                _gameState.Status  = GameStatus.Dying;
                return;
            }

            if (_settings.Coins.Contains(newPacMan.Location))
            {
                var newCollectedCoins = new List<CellLocation>(_collectedCoins)
                    {
                        (newPacMan.Location)
                    };
                _collectedCoins = newCollectedCoins;
                Score += 10;
                _soundSet.Chomp();
            }

        }

        private void UpdateStates(DateTime now)
        {
            
            if (_gameState.Status == GameStatus.Dead)
            {
                return;
            }
            
            if (_gameState.Status == GameStatus.Dying)
            {
                if (now >= _gameState.TimeToChangeState)
                {
                    _gameState.Status = GameStatus.Respawning;
                    _gameState.TimeToChangeState = now.AddSeconds(4);
                }

                return;
            }

            if (_gameState.Status == GameStatus.Respawning)
            {
                if (now >= _gameState.TimeToChangeState)
                {
                    _gameState.Status = GameStatus.Alive;
                    _gameState.TimeToChangeState = null;
                    PacMan = PacMan.SetToHome();
                    ApplyToGhosts(ghost => ghost.SetToHome());
                }

                return;
            }
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
        

    }
}
