using System;
using System.Threading.Tasks;

namespace NPacMan.Game.Tests
{
    public class TestGameClock : IGameClock
    {
        private Func<DateTime, Task>? _action;

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void Subscribe(Func<DateTime, Task> action)
        {
            _action = action;
        }

        public async Task Tick()
        {
            await Tick(DateTime.UtcNow);
        }

        public async Task Tick(DateTime now)
        {
            if (_action is null)
                return;

            await _action.Invoke(now);
        }
    }
}