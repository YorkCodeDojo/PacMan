using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Net.Http.Headers;
using System.Xml.XPath;

namespace NPacMan.Game
{

    public enum PacManStatus
    {
        Alive,
        Dying,
        Respawning,
        Dead
    }
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
            var board = @" XXXXXXXXXXXXXXXXXXXXXXXXXXXX
 X............XX............X
 X.XXXX.XXXXX.XX.XXXXX.XXXX.X
 X.X  X.X   X.XX.X   X.X  X.X
 X.XXXX.XXXXX.XX.XXXXX.XXXX.X
 X..........................X
 X.XXXX.XX.XXXXXXXX.XX.XXXX.X
 X.XXXX.XX.XXXXXXXX.XX.XXXX.X
 X......XX....XX....XX......X
 XXXXXX.XXXXX XX XXXXX.XXXXXX
      X.XXXXX XX XXXXX.X     
      X.XX          XX.X     
      X.XX   B      XX.X     
 XXXXXX.XX  XXXXXX  XX.XXXXXX
T      .    X IPCX    .      T
 XXXXXX.XX  XXXXXX  XX.XXXXXX
      X.XX          XX.X     
      X.XX          XX.X     
      X.XX XXXXXXXX XX.X
 XXXXXX.XX XXXXXXXX XX.XXXXXX
 X............XX............X
 X.XXXX.XXXXX.XX.XXXXX.XXXX.X
 X.XXXX.XXXXX.XX.XXXXX.XXXX.X
 X...XX.......►........XX...X
 XXX.XX.XX.XXXXXXXX.XX.XX.XXX
 XXX.XX.XX.XXXXXXXX.XX.XX.XXX
 X......XX....XX....XX......X
 X.XXXXXXXXXX.XX.XXXXXXXXXX.X
 X.XXXXXXXXXX.XX.XXXXXXXXXX.X
 X..........................X
 XXXXXXXXXXXXXXXXXXXXXXXXXXXX";

            return new Game(new GameClock(), GameSettingsLoader.Load(board));
        }

        public PacMan PacMan { get; private set; }
        public IReadOnlyCollection<(int x, int y)> Coins
            => _settings.Coins.Except(_collectedCoins).ToList().AsReadOnly();
        public IReadOnlyCollection<(int x, int y)> Walls
            => _settings.Walls;

        public int Width
            => _settings.Width;

        public int Height
            => _settings.Height;

        public int Lives
            => PacMan.Lives;

        public IReadOnlyDictionary<string, Ghost> Ghosts => _ghosts;

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
            return Ghosts.Values.Any(ghost => ghost.X == PacMan.X && ghost.Y == PacMan.Y);
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
    }
}
