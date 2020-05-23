using NPacMan.Game;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NPacMan.SharedUi;

namespace NPacMan.UI
{
    public partial class Form1 : Form
    {
        private readonly Timer _renderLoop = new Timer();
        private readonly BoardRenderer _boardRenderer = new BoardRenderer();
        private readonly GraphicsBuffers _graphicsBuffers;
        private readonly Game.Game _game;

        public Form1()
        {
            InitializeComponent();

            var soundSet = new SoundSet();

            _game = Game.Game.Create()
                             .Subscribe(GameNotification.Beginning, soundSet.Beginning)
                             .Subscribe(GameNotification.EatCoin, soundSet.Chomp)
                             .Subscribe(GameNotification.Respawning, soundSet.Death)
                             .Subscribe(GameNotification.EatFruit, soundSet.EatFruit)
                             .Subscribe(GameNotification.EatGhost, soundSet.EatGhost)
                             .Subscribe(GameNotification.ExtraPac, soundSet.ExtraPac)
                             .Subscribe(GameNotification.Intermission, soundSet.Intermission)
                             .StartGame();

            _graphicsBuffers = new GraphicsBuffers(this) { ShowFps = true };

            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "PacMan";
            this.WindowState = FormWindowState.Maximized;
            this.KeyDown += Form1_KeyDown;

            _renderLoop.Interval = 16; //40fps
            _renderLoop.Enabled = true;
            _renderLoop.Tick += _renderLoop_Tick;
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
                _boardRenderer.RenderStart(_game);
                _graphicsBuffers.RenderBackgroundUpdate(_boardRenderer);
            }
            catch (System.ObjectDisposedException)
            {
                // App closing - this.CreateGraphics() will fail
            }
        }
    }
}
