using NPacMan.Game;
using System.IO.Pipes;
using System.Linq;
using System.Text.Json;

namespace NPacMan.UI.Bots
{
    class BotConnector
    {
        private Game.Game? _game;
        private NamedPipeServerStream? _pipeServer;
        private StreamString? _pipeStream;
        private object _lock = new object();
        public void Initialise(Game.Game game)
        {
            _game = game;

            var board = new BotBoard
            {
                Portals = game.Portals.Select(kv => new BotPortal { Entry = kv.Key, Exit = kv.Value }),
                Height = game.Height,
                Width = game.Width,
                Walls = game.Walls,
            };
            var json = JsonSerializer.Serialize(board);

            _pipeServer = new NamedPipeServerStream("pacmanbot", PipeDirection.InOut, 1);
            _pipeServer.WaitForConnection();
            _pipeStream = new StreamString(_pipeServer);
            _pipeStream.WriteString("initialise:" + json);
        }

        public Direction? NextMove()
        {
            var botGame = new BotGame
            {
                Coins = _game!.Coins,
                Doors = _game.Doors,
                PowerPills = _game.PowerPills,
                Lives = _game.Lives,
                Score = _game.Score,
                PacMan = new Bots.BotPacMan { Location = _game.PacMan.Location, CurrentDirection = _game.PacMan.Direction },
                Ghosts = _game.Ghosts.Values.Select(g => new BotGhost { Edible = g.Edible, Location = g.Location, Name = g.Name }),
                Board = new BotBoard
                {
                    Portals = _game.Portals.Select(kv => new BotPortal { Entry = kv.Key, Exit = kv.Value }),
                    Height = _game.Height,
                    Width = _game.Width,
                    Walls = _game.Walls,
                }
            };

            var json = JsonSerializer.Serialize(botGame);

            var nextDirection = string.Empty;
            lock (_lock)
            {
                _pipeStream!.WriteString("play:" + json);

                nextDirection = _pipeStream.ReadString();
            }

            switch (nextDirection)
            {
                case "left":
                    return Direction.Left;
                case "right":
                    return Direction.Right;
                case "up":
                    return Direction.Up;
                case "down":
                    return Direction.Down;
                default:
                    return null;
            }
        }
    }
}
