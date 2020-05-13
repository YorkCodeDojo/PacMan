using System;
using System.Collections.Generic;

namespace NPacMan.Game
{
    public enum GameAction
    {
        EatCoin,
        EatFruit,
        Beginning,
        Dying,
        Respawning,
        Reborn,
        Intermission,
        ExtraPac,
        EatGhost,
    }

    public class GameNotifications
    {
        private readonly Dictionary<GameAction, List<Action>> _registrations = new Dictionary<GameAction, List<Action>>();

        public void Subscribe(GameAction gameAction, Action action)
        {
            if (!_registrations.TryGetValue(gameAction, out var actions))
            {
                actions = new List<Action>();
                _registrations[gameAction] = actions;
            }
            actions.Add(action);
        }

        public void Publish(GameAction gameAction)
        {
            if (_registrations.TryGetValue(gameAction, out var actions))
            {
                foreach (var action in actions)
                {
                    action();
                }
            }
        }
    }
}
