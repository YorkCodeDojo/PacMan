﻿using Automatonymous;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NPacMan.Game
{
    public class Game
    {

        private readonly IGameClock _gameClock;

        private readonly IGameSettings _settings;

        private readonly GameNotifications _gameNotifications = new GameNotifications();

        private readonly IReadOnlyGameState _gameState;

        private readonly GameStateMachine _gameStateMachine;

        private readonly InstanceLift<GameStateMachine> _gameStateMachineInstance;

        public Game(IGameClock gameClock, IGameSettings settings)
        {
            _gameClock = gameClock;
            _settings = settings;
            var actions = new Actions(settings, _gameNotifications);

            _gameStateMachine = new GameStateMachine(actions, settings, this);
            var gameState = new GameState(settings);
            _gameState = gameState;
            _gameStateMachineInstance = _gameStateMachine.CreateInstanceLift(gameState);
            WallsAndDoors = _settings.Walls.Union(_settings.Doors).ToList().AsReadOnly();
            Walls= _settings.Walls;
            _fruitForLevel = new[] { new Fruit(_settings.Fruit, FruitType.Cherry) };
            GhostHouseMiddle = CalculateMiddleOfGhostHouse(settings.GhostHouse);
        }

        private CellLocation CalculateMiddleOfGhostHouse(IReadOnlyCollection<CellLocation> ghostHouse)
        {
            var middleX = (int)ghostHouse.Average(x => x.X);
            var middleY = (int)ghostHouse.Average(x => x.Y);
            
            return (middleX, middleY);
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
            var gameSettings = GameSettingsLoader.LoadFromResource();

            return new Game(new GameClock(), gameSettings);
        }

        public IReadOnlyCollection<CellLocation> Coins
            => _gameState.RemainingCoins;

        internal IReadOnlyCollection<CellLocation> StartingCoins
            => _settings.Coins;

        public IReadOnlyCollection<CellLocation> PowerPills
            => _gameState.RemainingPowerPills;

        public IReadOnlyCollection<CellLocation> WallsAndDoors { get; }

        public IReadOnlyCollection<CellLocation> Walls { get; }

        public IReadOnlyCollection<CellLocation> Doors
            => _settings.Doors;

        public IReadOnlyDictionary<CellLocation, CellLocation> Portals
            => _settings.Portals;

        public IReadOnlyCollection<CellLocation> GhostHouse
            => _settings.GhostHouse;

        internal CellLocation GhostHouseMiddle { get; }

        public int Width
            => _settings.Width;

        public int Height
            => _settings.Height;

        public int Lives
            => _gameState.Lives;

        public int Score => _gameState.Score;

        public int HighScore => _gameState.HighScore;

        public int Level => _gameState.Level;


        public int PointsForEatingLastGhost => _gameState.PointsForEatingLastGhost;

        public PacMan PacMan => _gameState.PacMan;

        public IReadOnlyDictionary<string, Ghost> Ghosts
            => _gameState.GhostsVisible ? _gameState.Ghosts : new Dictionary<string, Ghost>();

        

        public GameStatus Status => _gameState.Status switch
        {
            nameof(GameStateMachine.Initial) => GameStatus.Initial,
            nameof(GameStateMachine.AttractMode) => GameStatus.AttractMode,
            nameof(GameStateMachine.GhostChase) => GameStatus.Alive,
            nameof(GameStateMachine.Scatter) => GameStatus.Alive,
            nameof(GameStateMachine.Frightened) => GameStatus.Alive,
            nameof(GameStateMachine.EatingGhost) => GameStatus.Alive,
            nameof(GameStateMachine.Dying) => GameStatus.Dying,
            nameof(GameStateMachine.Respawning) => GameStatus.Respawning,
            nameof(GameStateMachine.Dead) => GameStatus.Dead,
            nameof(GameStateMachine.ChangingLevel) => GameStatus.ChangingLevel,
            _ => throw new NotImplementedException($"No map for status '{_gameState.Status}'")
        };

        public Fruit[] Fruits 
        {
            get 
            {
                if (_gameState.FruitVisible)
                {
                    if (_fruitForLevel[0].Type != _gameState.FruitTypeToShow)
                    {
                        _fruitForLevel[0] = new Fruit(_settings.Fruit, _gameState.FruitTypeToShow);
                    }
                    return _fruitForLevel;                        
                }   
                else
                {
                    return Array.Empty<Fruit>();;
                }                 
            }
        }


        private readonly Fruit[] _fruitForLevel;

        private async Task Tick(DateTime now)
        {
            await _gameStateMachineInstance.Raise(_gameStateMachine.Tick, new Tick(now));
        }
        public async Task ChangeDirection(Direction direction)
        {
            await _gameStateMachineInstance.Raise(_gameStateMachine.PlayersWishesToChangeDirection, new PlayersWishesToChangeDirection(direction));
        }

        public async Task PressStart()
        {
            await _gameStateMachineInstance.Raise(_gameStateMachine.PressStart, new PressStart());
        }
    }
}
