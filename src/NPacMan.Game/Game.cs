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
        private List<CellLocation> _collectedPowerPills;
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
            _collectedPowerPills = new List<CellLocation>();
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

        public IReadOnlyCollection<CellLocation> PowerPills
            => _settings.PowerPills.Except(_collectedPowerPills).ToList().AsReadOnly();

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
            nameof(GameStateMachine.GhostChase) => GameStatus.Alive,
            nameof(GameStateMachine.Scatter) => GameStatus.Alive,
            nameof(GameStateMachine.Frightened) => GameStatus.Alive,
            nameof(GameStateMachine.Dying) => GameStatus.Dying,
            nameof(GameStateMachine.Respawning) => GameStatus.Respawning,
            nameof(GameStateMachine.Dead) => GameStatus.Dead,
            _ => throw new NotImplementedException($"No map for status '{_gameState.Status}'")
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

        private bool TryHasDied(out Ghost? ghost)
        {
            ghost = Ghosts.Values.FirstOrDefault(ghost => ghost.Location.X == PacMan.Location.X && ghost.Location.Y == PacMan.Location.Y);

            return !(ghost is null);
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


        Task IGameActions.MoveGhosts(BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine)
        {
            return MoveGhosts(context, gameStateMachine);
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

        void IGameActions.MakeGhostsEdible()
        {
            ApplyToGhosts(ghost => ghost.SetToEdible());
        }

        
        void IGameActions.MakeGhostsNotEdible()
        {
            ApplyToGhosts(ghost => ghost.SetToNotEdible());
        }

        void IGameActions.MovePacManHome()
        {
            PacMan = PacMan.SetToHome();
        }


        void IGameActions.SendGhostHome(Ghost ghostToSendHome)
        {
            ApplyToGhosts(ghost => {
                if(ghost.Name == ghostToSendHome.Name){
                    ghost = ghost.SetToHome();
                }
                return ghost;
            });
        }

        async Task IGameActions.MovePacMan(BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine)
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
            var died = false;
            if(TryHasDied(out var ghost))
            {
                died = !ghost!.Edible;

                await context.Raise(gameStateMachine.GhostCollision, new GhostCollision(ghost));
            }

            if (died)
            {
                
            }
            else if (Coins.Contains(newPacMan.Location))
            {
                var newCollectedCoins = new List<CellLocation>(_collectedCoins)
                    {
                        (newPacMan.Location)
                    };
                _collectedCoins = newCollectedCoins;

                await context.Raise(gameStateMachine.CoinEaten);
            }
            else if (PowerPills.Contains(newPacMan.Location))
            {
                var newCollectedPowerPills = new List<CellLocation>(_collectedPowerPills)
                {
                    (newPacMan.Location)
                };
                _collectedPowerPills = newCollectedPowerPills;

                await context.Raise(gameStateMachine.PowerPillEaten);
            }
        }

        private async Task MoveGhosts(BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine)
        {
            ApplyToGhosts(ghost => ghost.Move(this));
            if(TryHasDied(out var ghost))
            {
                await context.Raise(gameStateMachine.GhostCollision, new GhostCollision(ghost!));
            }
        }

    }
}
