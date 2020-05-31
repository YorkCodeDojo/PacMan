using System;
using System.Collections.Generic;
using System.Linq;

namespace PacManDebugger
{
    public class History
    {
        private Dictionary<string, HistoricEvent[]> _ghostHistory = new Dictionary<string, HistoricEvent[]>();

        public void AddHistoricEvent(string ghostName, int tickCounter, Location originalLocation, Location finalLocation)
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
    }
}
