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
        }

        public void Start()
        {
            Task.Run(() =>
            {
                _session.EnableProvider("PacManEventSource", Microsoft.Diagnostics.Tracing.TraceEventLevel.Always);

                _session.Source.Dynamic.AddCallbackForProviderEvent("PacManEventSource", "GameStarted", traceEvent =>
                {
                    var width = (int)traceEvent.PayloadByName("width");
                    var height = (int)traceEvent.PayloadByName("height");
                    var wallsJson = (string)traceEvent.PayloadByName("wallsJson");

                    var serializeOptions = new JsonSerializerOptions();
                    serializeOptions.Converters.Add(new CellLocationConverter());
                    var walls = JsonSerializer.Deserialize<CellLocation[]>(wallsJson, serializeOptions);

                    _board.UpdateDefinition(width, height, walls);
                });

                _session.Source.Dynamic.AddCallbackForProviderEvent("PacManEventSource", "GhostMoved", traceEvent =>
                {
                    var ghostName = (string)traceEvent.PayloadByName("ghostName");
                    var tickCounter = (int)traceEvent.PayloadByName("tickCounter");
                    var fromLocationX = (int)traceEvent.PayloadByName("fromLocationX");
                    var fromLocationY = (int)traceEvent.PayloadByName("fromLocationY");
                    var toLocationX = (int)traceEvent.PayloadByName("toLocationX");
                    var toLocationY = (int)traceEvent.PayloadByName("toLocationY");

                    _history.AddHistoricEvent(ghostName, tickCounter, new CellLocation(fromLocationX, fromLocationY), new CellLocation(toLocationX, toLocationY));
                });

                _session.Source.Dynamic.AddCallbackForProviderEvent("PacManEventSource", "PacManMoved", traceEvent =>
                {
                    var tickCounter = (int)traceEvent.PayloadByName("tickCounter");
                    var fromLocationX = (int)traceEvent.PayloadByName("fromLocationX");
                    var fromLocationY = (int)traceEvent.PayloadByName("fromLocationY");
                    var toLocationX = (int)traceEvent.PayloadByName("toLocationX");
                    var toLocationY = (int)traceEvent.PayloadByName("toLocationY");

                    _history.AddHistoricPacManEvent(tickCounter, new CellLocation(fromLocationX, fromLocationY), new CellLocation(toLocationX, toLocationY));
                });

                _session.Source.Dynamic.AddCallbackForProviderEvent("PacManEventSource", "PacManStateChanged", traceEvent =>
                {
                    var tickCounter = (int)traceEvent.PayloadByName("tickCounter");
                    var lives = (int)traceEvent.PayloadByName("lives");
                    var score = (int)traceEvent.PayloadByName("score");
                    var locationX = (int)traceEvent.PayloadByName("locationX");
                    var locationY = (int)traceEvent.PayloadByName("locationY");
                    var direction = (string)traceEvent.PayloadByName("direction");

                    _history.AddHistoricPacManEvent(tickCounter, new CellLocation(locationX, locationY), new CellLocation(locationX, locationY));
                });

                _session.Source.Process();
            });
        }

        public void Stop()
        {
            _session.Source.StopProcessing();
        }
    }
}
