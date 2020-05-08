using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

namespace NPacMan.Game
{
    public class Game
    {
        public int Score { get; private set; }

        private readonly IGameSettings _settings;
        private List<(int x, int y)> _collectedCoins;
        private Dictionary<string, Ghost> _ghosts;


        public Game(IGameClock gameClock, IGameSettings settings)
        {
            gameClock.Subscribe(Tick);
            _settings = settings;
            PacMan = settings.PacMan;
            _collectedCoins = new List<(int x, int y)>();
            _ghosts = settings.Ghosts.ToDictionary(x => x.Name, x => x);
        }

        public static Game Create()
        {
            var filename = "board.txt";
            var gameSettings = GameSettingsLoader.LoadFromFile(filename);

            return new Game(new GameClock(), gameSettings);
        }

        public PacMan PacMan { get; private set; }
        public IReadOnlyCollection<(int x, int y)> Coins
            => _settings.Coins.Except(_collectedCoins).ToList().AsReadOnly();
        public IReadOnlyCollection<(int x, int y)> Walls
            => _settings.Walls;

        public IReadOnlyCollection<CellLocation> Doors
            => _settings.Doors;

        public int Width
            => _settings.Width;

        public int Height
            => _settings.Height;

        public int Lives
            => PacMan.Lives;

        public IReadOnlyDictionary<string, Ghost> Ghosts
            => PacMan.Status == PacManStatus.Respawning ? (IReadOnlyDictionary<string, Ghost>)ImmutableDictionary<string, Ghost>.Empty : _ghosts;

        public void ChangeDirection(Direction direction)
        {
            PacMan = PacMan.WithNewDirection(direction);
        }

        private void Tick(DateTime now)
        {
            var newPacMan = PacMan.Transition(now);

            if (_settings.Portals.TryGetValue((newPacMan.X, newPacMan.Y), out var portal))
            {
                newPacMan = PacMan.WithNewX(portal.x).WithNewY(portal.y);
                newPacMan = newPacMan.Transition(now);
            }

            if (PacMan.Status == PacManStatus.Dying && newPacMan.Status == PacManStatus.Respawning)
            {
                ScatterAllGhosts();
            }

            if (PacMan.Status != PacManStatus.Dying)
            {
                MoveAllGhosts();
            }

            if (newPacMan.Status != PacManStatus.Alive)
            {
                PacMan = newPacMan;
                return;
            }

            if (HasDied())
            {
                PacMan = PacMan.Kill(now.AddSeconds(4));
                return;
            }

            if (!_settings.Walls.Contains((newPacMan.X, newPacMan.Y)))
            {
                PacMan = newPacMan;
            }

            if (HasDied())
            {
                PacMan = PacMan.Kill(now.AddSeconds(4));
                return;
            }

            if (_settings.Coins.Contains((newPacMan.X, newPacMan.Y)))
            {
                var newCollectedCoins = new List<(int, int)>(_collectedCoins)
                    {
                        (newPacMan.X, newPacMan.Y)
                    };
                _collectedCoins = newCollectedCoins;
                Score += 10;
            }

        }

        private bool HasDied()
        {
            return Ghosts.Values.Any(ghost => ghost.Location.X == PacMan.X && ghost.Location.Y == PacMan.Y);
        }

        private void MoveAllGhosts()
        {
            var newPositionOfGhosts = new Dictionary<string, Ghost>();
            foreach (var ghost in Ghosts.Values)
            {
                newPositionOfGhosts[ghost.Name] = ghost.Move(this);
            }

            _ghosts = newPositionOfGhosts;
        }

        private void ScatterAllGhosts()
        {
            var newPositionOfGhosts = new Dictionary<string, Ghost>();
            foreach (var ghost in Ghosts.Values)
            {
                newPositionOfGhosts[ghost.Name] = ghost.Scatter();
            }

            _ghosts = newPositionOfGhosts;
        }

    }
}
