using System;
using System.Collections.Generic;
using System.Linq;

namespace PacManDebugger
{
    public class History
    {
        private Dictionary<string, HistoricEvent[]> _ghostHistory = new Dictionary<string, HistoricEvent[]>();

        private HistoricEvent[] _pacmanHistory = new HistoricEvent[1000];

        public void AddHistoricEvent(string ghostName, int tickCounter, CellLocation originalLocation, CellLocation finalLocation)
        {
            if (!_ghostHistory.TryGetValue(ghostName, out var historicEvents))
            {
                historicEvents = new HistoricEvent[1000];
                _ghostHistory.Add(ghostName, historicEvents);
            }

            var historicEvent = new HistoricEvent(originalLocation, finalLocation, wasMoveEvent: true);
            historicEvents[tickCounter] = historicEvent;
        }

        internal string[] GhostNames() => _ghostHistory.Keys.ToArray();

        internal HistoricEvent GetHistoricEventForTickCount(string ghostName, int tickCount)
        {
            if (_ghostHistory.TryGetValue(ghostName, out var historicEvents))
            {
                while (!historicEvents[tickCount].WasMoveEvent && tickCount > 0)
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
            _pacmanHistory = new HistoricEvent[1000];
        }

        internal void AddHistoricPacManEvent(int tickCounter, CellLocation originalLocation, CellLocation finalLocation)
        {
            var historicEvent = new HistoricEvent(originalLocation, finalLocation, wasMoveEvent: true);
            _pacmanHistory[tickCounter] = historicEvent;
        }

        internal HistoricEvent GetHistoricPacManEventForTickCount(int tickCount)
        {
            return _pacmanHistory[tickCount];
        }
    }
}
