using NPacMan.Game;
using System.Linq;
using System.Text.Json;
using NPacMan.UI.Bots.Models;
using NPacMan.UI.Bots.Transports;

namespace NPacMan.UI.Bots
{
    class BotConnector
    {
        private Game.Game _game;
        private readonly IBotTransport _transport;

        public BotConnector(IBotTransport namedPipesTransport, Game.Game game)
        {
            _transport = namedPipesTransport;
            _game = game;
        }

        public void Initialise()
        {
            var board = new BotBoard
            {
                Portals = _game.Portals.Select(kv => new BotPortal { Entry = new BotLocation(kv.Key), Exit = new BotLocation(kv.Value) }),
                Height = _game.Height,
                Width = _game.Width,
                Walls = _game.Walls.Select(l => new BotLocation(l)),
            };
            var json = JsonSerializer.Serialize(board);
            var payload = $"initialise:{json}";

            _transport.SendCommand(payload);
        }

        public async void RequestNextMoveFromBot()
        {
            if (_game.Status == GameStatus.Alive)
            {
                var nextDirection = NextMove();
                if (nextDirection is object)
                {
                    await _game.ChangeDirection(nextDirection.Value);
                }
            }
        }

        private Direction? NextMove()
        {
            var botGame = new BotGame
            {
                Coins = _game!.Coins.Select(l => new BotLocation(l)),
                Doors = _game.Doors.Select(l => new BotLocation(l)),
                PowerPills = _game.PowerPills.Select(l => new BotLocation(l)),
                Lives = _game.Lives,
                Score = _game.Score,
                PacMan = new BotPacMan { Location = new BotLocation(_game.PacMan.Location), CurrentDirection = _game.PacMan.Direction },
                Ghosts = _game.Ghosts.Values.Select(g => new BotGhost { Edible = g.Edible, Location = new BotLocation(g.Location), Name = g.Name }),
                Board = new BotBoard
                {
                    Portals = _game.Portals.Select(kv => new BotPortal { Entry = new BotLocation(kv.Key), Exit = new BotLocation(kv.Value) }),
                    Height = _game.Height,
                    Width = _game.Width,
                    Walls = _game.Walls.Select(l => new BotLocation(l)),
                }
            };

            var json = JsonSerializer.Serialize(botGame);
                var payload = $"play:{json}";

            var nextDirection = _transport.SendCommand(payload);

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
