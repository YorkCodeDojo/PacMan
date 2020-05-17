﻿using Automatonymous;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace NPacMan.Game
{
    public class Game : IGameActions
    {
        public int Score => _gameState.Score;

        private readonly IGameClock _gameClock;
        private readonly IGameSettings _settings;
        private Dictionary<string, Ghost> _ghosts;

        private readonly GameNotifications _gameNotifications = new GameNotifications();

        private readonly GameState _gameState;

        private readonly GameStateMachine _gameStateMachine;

        public Game(IGameClock gameClock, IGameSettings settings)
        {
            _gameClock = gameClock;
            _settings = settings;
            PacMan = settings.PacMan;
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
            if (!Walls.Contains(nextSpace))
            {
                PacMan = PacMan.WithNewDirection(direction);
            }
        }

        private async Task Tick(DateTime now)
        {
            await _gameStateMachine.RaiseEvent(_gameState, _gameStateMachine.Tick, new Tick(now));
        }

        private IEnumerable<Ghost> GhostsCollidedWithPacMan()
        {
            return Ghosts.Values.Where(ghost => ghost.Location == PacMan.Location);
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

        Task IGameActions.MoveGhosts(BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine)
        {
            return MoveGhosts(context, gameStateMachine);
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
            ApplyToGhosts(ghost =>
            {
                if (ghost.Name == ghostToSendHome.Name)
                {
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

            var ghosts = GhostsCollidedWithPacMan();
            foreach (var ghost in ghosts)
            {
                await context.Raise(gameStateMachine.GhostCollision, new GhostCollision(ghost));
            }

            if (Coins.Contains(newPacMan.Location))
            {
                await context.Raise(gameStateMachine.CoinCollision, new CoinCollision(newPacMan.Location));
            }

            if (PowerPills.Contains(newPacMan.Location))
            {
                await context.Raise(gameStateMachine.PowerPillCollision, new PowerPillCollision(newPacMan.Location));
            }
        }

        private async Task MoveGhosts(BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine)
        {
            ApplyToGhosts(ghost => ghost.Move(this));

            var ghosts = GhostsCollidedWithPacMan();
            foreach (var ghost in ghosts)
            {
                await context.Raise(gameStateMachine.GhostCollision, new GhostCollision(ghost));
            }
        }

    }
}
