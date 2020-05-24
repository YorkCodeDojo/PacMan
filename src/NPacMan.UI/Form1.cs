using GreenPipes.Filters;
using NPacMan.Game;
using NPacMan.UI.Bots;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace NPacMan.UI
{
    public partial class Form1 : Form
    {
        private readonly Timer _renderLoop = new Timer();
        private readonly BoardRenderer _boardRenderer = new BoardRenderer();
        private readonly GraphicsBuffers _graphicsBuffers;
        private readonly Game.Game _game;
        private readonly NamedPipeServerStream _pipeServer;
        private readonly StreamString _pipeStream;
        private readonly IBot _bot;

        public Form1()
        {
            InitializeComponent();

            var soundSet = new SoundSet();

            _game = Game.Game.Create()
                             //.Subscribe(GameNotification.Beginning, soundSet.Beginning)
                             //.Subscribe(GameNotification.EatCoin, soundSet.Chomp)
                             //.Subscribe(GameNotification.Respawning, soundSet.Death)
                             //.Subscribe(GameNotification.EatFruit, soundSet.EatFruit)
                             //.Subscribe(GameNotification.EatGhost, soundSet.EatGhost)
                             //.Subscribe(GameNotification.ExtraPac, soundSet.ExtraPac)
                             //.Subscribe(GameNotification.Intermission, soundSet.Intermission)
                             .Subscribe(GameNotification.PreTick, BeforeTick)
                             .StartGame();

            _pipeServer = new NamedPipeServerStream("pacmanbot", PipeDirection.InOut, 1);
            _pipeServer.WaitForConnection();
            _pipeStream = new StreamString(_pipeServer);
            _pipeStream.WriteString("I am the one true server!");

            _bot = new GreedyBot(_game);

            _graphicsBuffers = new GraphicsBuffers(this) { ShowFps = true };

            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "PacMan";
            this.WindowState = FormWindowState.Maximized;
            this.KeyDown += Form1_KeyDown;

            _renderLoop.Interval = 16; //40fps
            _renderLoop.Enabled = true;
            _renderLoop.Tick += _renderLoop_Tick;
        }

        private object _lock = new object();

        private async void BeforeTick()
        {
            if (_game.Status == GameStatus.Alive)
            {
                var botGame = new BotGame
                {
                    Coins = _game.Coins,
                    Doors = _game.Doors,
                    Portals = _game.Portals.Select(kv => new BotPortal { Entry = kv.Key, Exit = kv.Value }),
                    PowerPills = _game.PowerPills,
                    Height = _game.Height,
                    Width = _game.Width,
                    Lives = _game.Lives,
                    Score = _game.Score,
                    Walls = _game.Walls,
                    PacMan = _game.PacMan.Location,
                    Ghosts = _game.Ghosts.Values.Select(g => new BotGhost { Edible = g.Edible, Location = g.Location, Name = g.Name }),
                };

                var json = JsonSerializer.Serialize(botGame);

                var nextDirection = string.Empty;
                lock (_lock)
                {
                    _pipeStream.WriteString(json);

                    nextDirection = _pipeStream.ReadString();
                }

                switch (nextDirection)
                {
                    case "left":
                        await _game.ChangeDirection(Direction.Left);
                        break;
                    case "right":
                        await _game.ChangeDirection(Direction.Right);
                        break;
                    case "up":
                        await _game.ChangeDirection(Direction.Up);
                        break;
                    case "down":
                        await _game.ChangeDirection(Direction.Down);
                        break;
                    default:
                        break;
                }
            }
        }

        private static readonly IReadOnlyDictionary<Keys, Direction> _keysMap
            = new Dictionary<Keys, Direction>{
                {Keys.Up, Direction.Up},
                {Keys.Down, Direction.Down},
                {Keys.Left, Direction.Left},
                {Keys.Right, Direction.Right},
            };
        private async void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (_keysMap.TryGetValue(e.KeyCode, out var direction))
            {
                await _game.ChangeDirection(direction);
            }
        }

        private void _renderLoop_Tick(object? sender, EventArgs e)
        {
            try
            {
                var g = _graphicsBuffers.GetBitmapBuffer(_game.Width * Sprites.PixelGrid,
                    (_game.Height + 5) * Sprites.PixelGrid);

                _boardRenderer.RenderScore(g, _game);

                g.TranslateTransform(0, 3 * Sprites.PixelGrid);
                _boardRenderer.RenderWalls(g, _game);
                _boardRenderer.RenderCoins(g, _game);
                _boardRenderer.RenderPowerPills(g, _game);
                _boardRenderer.RenderPacMan(g, _game);
                _boardRenderer.RenderGhosts(g, _game);
                g.ResetTransform();

                g.TranslateTransform(0, (3 + _game.Height) * Sprites.PixelGrid);
                _boardRenderer.RenderLives(g, _game);
                g.ResetTransform();

                _graphicsBuffers.Render();
            }
            catch (System.ObjectDisposedException)
            {
                // App closing - this.CreateGraphics() will fail
            }
        }
    }
}
