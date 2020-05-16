using System;
using System.Threading;
using System.Threading.Tasks;

namespace NPacMan.Game
{
    public class GameClock : IGameClock, IDisposable
    {
        private Func<DateTime, Task>? _action;
        private readonly Timer _timer;

        public GameClock()
        {
            _timer = new Timer((state) => _action?.Invoke(DateTime.UtcNow), null, 0, 200);
        }

        public void Subscribe(Func<DateTime, Task> action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}