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
        private List<CellLocation> _collectedCoins;
        private Dictionary<string, Ghost> _ghosts;


        public Game(IGameClock gameClock, IGameSettings settings)
        {
            gameClock.Subscribe(Tick);
            _settings = settings;
            PacMan = settings.PacMan;
            _collectedCoins = new List<CellLocation>();
            _ghosts = settings.Ghosts.ToDictionary(x => x.Name, x => x);
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

            if (_settings.Portals.TryGetValue(newPacMan.Location, out var portal))
            {
                newPacMan = PacMan.WithNewX(portal.X).WithNewY(portal.Y);
                newPacMan = newPacMan.Transition(now);
            }

            if (PacMan.Status == PacManStatus.Respawning && newPacMan.Status == PacManStatus.Alive)
            {
                ApplyToGhosts(ghost => ghost.SetToHome());
            }

            if (PacMan.Status == PacManStatus.Alive)
            {
                ApplyToGhosts(ghost => ghost.Move(this));
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

            if (!_settings.Walls.Contains(newPacMan.Location))
            {
                PacMan = newPacMan;
            }

            if (HasDied())
            {
                PacMan = PacMan.Kill(now.AddSeconds(4));
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
