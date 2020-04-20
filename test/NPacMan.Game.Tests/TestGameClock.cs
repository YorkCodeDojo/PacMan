using System;
using System.Collections.Generic;

namespace NPacMan.Game.Tests
{
    public class TestGameClock : IGameClock
    {
        private List<Action> _actions = new List<Action>();

        public void Subscribe(Action action)
        {
            _actions.Add(action);
        }

        public void Tick()
        {
            _actions.ForEach(x => x());
        }
    }
}