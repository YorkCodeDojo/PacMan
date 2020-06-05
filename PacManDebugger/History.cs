using System;
using System.Collections.Generic;
using System.Linq;

namespace PacManDebugger
{
    public class History
    {
        private Dictionary<string, HistoricEvent[]> _ghostHistory = new Dictionary<string, HistoricEvent[]>();

        private HistoricPacManMovementEvent[] _pacmanMovements = new HistoricPacManMovementEvent[1000];
        private HistoricPacManStateChangeEvent[] _pacmanStateChanges = new HistoricPacManStateChangeEvent[1000];

        internal string[] GhostNames() => _ghostHistory.Keys.ToArray();

        internal HistoricEvent GetHistoricEventForTickCount(string ghostName, int tickCount)
        {
            if (_ghostHistory.TryGetValue(ghostName, out var historicEvents))
            {
                while (!historicEvents[tickCount].EventSet && tickCount > 0)
                {
                    tickCount--;
                }

                return historicEvents[tickCount];
            }
            else
            {
                throw new Exception("Unknown ghost name - " + ghostName);
            }
        }

        internal void Clear()
        {
            _ghostHistory.Clear();
            _pacmanMovements = new HistoricPacManMovementEvent[1000];
            _pacmanStateChanges = new HistoricPacManStateChangeEvent[1000];
        }

        internal void AddHistoricPacManMovementEvent(int tickCounter, CellLocation originalLocation, CellLocation finalLocation)
        {
            var historicEvent = new HistoricPacManMovementEvent(originalLocation, finalLocation);
            _pacmanMovements[tickCounter] = historicEvent;
        }

        internal void AddHistoricPacManStateChangedEvent(int tickCounter, int lives, int score, string direction)
        {
            var historicEvent = new HistoricPacManStateChangeEvent(lives, score, direction);
            _pacmanStateChanges[tickCounter] = historicEvent;
        }

        public void AddHistoricGhostMovementEvent(string ghostName, int tickCounter, CellLocation originalLocation, CellLocation finalLocation, string direction)
        {
            if (!_ghostHistory.TryGetValue(ghostName, out var historicEvents))
            {
                historicEvents = new HistoricEvent[1000];
                _ghostHistory.Add(ghostName, historicEvents);
            }

            var historicEvent = new HistoricEvent(originalLocation, finalLocation, eventSet: true);
            historicEvents[tickCounter] = historicEvent;
        }

        internal void AddHistoricGhostStateChangedEvent(string ghostName, int tickCounter, string strategy, bool edible, string direction)
        {
        }

        internal HistoricPacManMovementEvent GetHistoricPacManMovementEventForTickCount(int tickCount)
        {
            while (!_pacmanMovements[tickCount].EventSet && tickCount > 0)
            {
                tickCount--;
            }

            return _pacmanMovements[tickCount];
        }

        internal HistoricPacManStateChangeEvent GetHistoricPacManStateChangeEventForTickCount(int tickCount)
        {
            while (!_pacmanStateChanges[tickCount].EventSet && tickCount > 0)
            {
                tickCount--;
            }

            return _pacmanStateChanges[tickCount];
        }
    }
}
