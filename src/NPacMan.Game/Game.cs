using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Automatonymous;

namespace NPacMan.Game
{
    internal class GameState
    {
        public GameState(IGameSettings settings)
        {
            Lives = settings.InitialLives;
            Status = settings.InitialGameStatus switch
            {
                GameStatus.Initial => nameof(GameStateMachine.Initial),
                GameStatus.Alive => nameof(GameStateMachine.Alive),
                GameStatus.Dying => nameof(GameStateMachine.Dying),
                GameStatus.Respawning => nameof(GameStateMachine.Respawning),
                GameStatus.Dead => nameof(GameStateMachine.Dead),
                _ => throw new NotImplementedException($"No map for InitialGameStatus '{settings.InitialGameStatus}")
            };
            Score = 0;
            GhostsVisible = true;
            TimeToChangeState = null;
        }

        public string Status { get; set; } = null!;

        public DateTime? TimeToChangeState { get; set; }

        public int Lives { get; set; }

        public bool GhostsVisible { get; set; }

        public int Score { get; set; }
    }

    internal interface IGameActions
    {
        void MoveGhostsHome();
        void ShowGhosts(GameState gameState);
        void ScatterGhosts();
        void GhostToChase();
        Task MovePacMan(DateTime now);
        Task MoveGhosts(DateTime now);
        void HideGhosts(GameState gameState);
        void MovePacManHome();
    }

    internal class GameStateMachine :
               AutomatonymousStateMachine<GameState>
    {
        public GameStateMachine(IGameActions game, IGameSettings settings)
        {
            InstanceState(x => x.Status);

            Initially(
                When(Tick)
                    .Then(context => game.MoveGhostsHome())
                    .Then(context => game.ShowGhosts(context.Instance))
                    .Then(context => game.ScatterGhosts())
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(settings.InitialScatterTimeInSeconds))
                    .TransitionTo(Scatter));

            During(Scatter,
                 When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(settings.ChaseTimeInSeconds))
                     .Then(context => game.GhostToChase())
                     .TransitionTo(Alive));

            During(Alive,
                 When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(settings.InitialScatterTimeInSeconds))
                     .Then(context => game.ScatterGhosts())
                     .TransitionTo(Scatter));

            During(Scatter, Alive,
                When(Tick)
                    .ThenAsync(async context => await game.MoveGhosts(context.Data.Now))
                    .Then(context => game.MovePacMan(context.Data.Now)),
                When(CoinEaten)
                    .Then(context => context.Instance.Score += 10),
                    //.Then(context => game._soundSet.Chomp()),
                When(PacManCaughtByGhost)
                    .Then(context => context.Instance.Lives -= 1)
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(4))
                    .TransitionTo(Dying));

            During(Dying,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => game.HideGhosts(context.Instance))
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(4))
                    //.Then(context => game._soundSet.Death())
                    .IfElse(context => context.Instance.Lives > 0,
                        binder => binder.TransitionTo(Respawning),
                        binder => binder.Finalize()));

            During(Respawning,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(4))
                    .Then(context => game.MoveGhostsHome())
                    .Then(context => game.MovePacManHome())
                    .Then(context => game.ShowGhosts(context.Instance))
                    .TransitionTo(Alive));

            During(Dead, Ignore(Tick));
        }


        public State Alive { get; private set; } = null!;

        public State Scatter { get; private set; } = null!;
        public State Dying { get; private set; } = null!;
        public State Respawning { get; private set; } = null!;
        public State Dead { get; private set; } = null!;
        public Event<Tick> Tick { get; private set; } = null!;
        public Event<Tick> PacManCaughtByGhost { get; private set; } = null!;
        public Event CoinEaten { get; private set; } = null!;
    }


    internal class Tick
    {
        public DateTime Now { get; set; }
    }

    public class Game : IGameActions
    {
        public int Score => _gameState.Score;

        private readonly IGameSettings _settings;
        private List<CellLocation> _collectedCoins;
        private Dictionary<string, Ghost> _ghosts;

        private ISoundSet _soundSet;

        private GameState _gameState;

        private GameStateMachine _gameStateMachine;
        public Game(IGameClock gameClock, IGameSettings settings, ISoundSet soundSet)
        {
            _settings = settings;
            PacMan = settings.PacMan;
            _collectedCoins = new List<CellLocation>();
            _ghosts = settings.Ghosts.ToDictionary(x => x.Name, x => x);
            _soundSet = soundSet;
            _gameStateMachine = new GameStateMachine(this, settings);
            _gameState = new GameState(settings);

            // Play the beginning sound
            gameClock.Subscribe(Tick);
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
            PacMan = PacMan.WithNewDirection(direction);
        }

        private async Task Tick(DateTime now)
        {
            await _gameStateMachine.RaiseEvent(_gameState, _gameStateMachine.Tick, new Tick { Now = now });
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
                await _gameStateMachine.RaiseEvent(_gameState, _gameStateMachine.PacManCaughtByGhost, new Tick { Now = now });
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
                await _gameStateMachine.RaiseEvent(_gameState, _gameStateMachine.PacManCaughtByGhost, new Tick { Now = now });
            }
        }
    }
}
