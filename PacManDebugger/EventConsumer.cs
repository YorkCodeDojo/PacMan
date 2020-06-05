using Microsoft.Diagnostics.Tracing.Session;
using System.Text.Json;
using System.Threading.Tasks;

namespace PacManDebugger
{
    class EventConsumer
    {
        private readonly History _history;
        private readonly Board _board;
        private readonly TraceEventSession _session;

        public EventConsumer(History history, Board board)
        {
            _history = history;
            _board = board;
            _session = new TraceEventSession("PacManDebugger");

            _session.EnableProvider("PacManEventSource", Microsoft.Diagnostics.Tracing.TraceEventLevel.Always);

            //1
            _session.Source.Dynamic.AddCallbackForProviderEvent("PacManEventSource", "GameStarted", traceEvent =>
            {
                var width = (int)traceEvent.PayloadByName("width");
                var height = (int)traceEvent.PayloadByName("height");
                var wallsJson = (string)traceEvent.PayloadByName("wallsJson");

                var serializeOptions = new JsonSerializerOptions();
                serializeOptions.Converters.Add(new CellLocationConverter());
                var walls = JsonSerializer.Deserialize<CellLocation[]>(wallsJson, serializeOptions);

                _history.Clear();
                _board.UpdateDefinition(width, height, walls);
            });

            //2
            _session.Source.Dynamic.AddCallbackForProviderEvent("PacManEventSource", "GhostMoved", traceEvent =>
            {
                var ghostName = (string)traceEvent.PayloadByName("ghostName");
                var tickCounter = (int)traceEvent.PayloadByName("tickCounter");
                var fromLocationX = (int)traceEvent.PayloadByName("fromLocationX");
                var fromLocationY = (int)traceEvent.PayloadByName("fromLocationY");
                var toLocationX = (int)traceEvent.PayloadByName("toLocationX");
                var toLocationY = (int)traceEvent.PayloadByName("toLocationY");
                var direction = (string)traceEvent.PayloadByName("direction");

                _history.AddHistoricGhostMovementEvent(ghostName, tickCounter, new CellLocation(fromLocationX, fromLocationY), new CellLocation(toLocationX, toLocationY), direction);
            });

            //4
            _session.Source.Dynamic.AddCallbackForProviderEvent("PacManEventSource", "GhostChangedState", traceEvent =>
            {
                var ghostName = (string)traceEvent.PayloadByName("ghostName");
                var tickCounter = (int)traceEvent.PayloadByName("tickCounter");
                var strategy = (string)traceEvent.PayloadByName("strategy");
                var edible = (bool)traceEvent.PayloadByName("edible");
                var direction = (string)traceEvent.PayloadByName("direction");

                _history.AddHistoricGhostStateChangedEvent(ghostName,
                                                           tickCounter,
                                                           strategy,
                                                           edible,
                                                           direction);
            });

            //5
            _session.Source.Dynamic.AddCallbackForProviderEvent("PacManEventSource", "PacManMoved", traceEvent =>
            {
                var tickCounter = (int)traceEvent.PayloadByName("tickCounter");
                var fromLocationX = (int)traceEvent.PayloadByName("fromLocationX");
                var fromLocationY = (int)traceEvent.PayloadByName("fromLocationY");
                var toLocationX = (int)traceEvent.PayloadByName("toLocationX");
                var toLocationY = (int)traceEvent.PayloadByName("toLocationY");

                _history.AddHistoricPacManMovementEvent(tickCounter, new CellLocation(fromLocationX, fromLocationY), new CellLocation(toLocationX, toLocationY));
            });

            //6
            _session.Source.Dynamic.AddCallbackForProviderEvent("PacManEventSource", "PacManStateChanged", traceEvent =>
            {
                var tickCounter = (int)traceEvent.PayloadByName("tickCounter");
                var lives = (int)traceEvent.PayloadByName("lives");
                var score = (int)traceEvent.PayloadByName("score");
                var direction = (string)traceEvent.PayloadByName("direction");

                _history.AddHistoricPacManStateChangedEvent(tickCounter,
                                                            lives,
                                                            score,
                                                            direction);
            });
        }

        public void Start()
        {
            Task.Run(() =>
            {
                _session.Source.Process();
            });
        }

        public void Stop()
        {
            _session.Source.StopProcessing();
        }
    }
}
