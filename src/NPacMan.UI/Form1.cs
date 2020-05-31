using System;
using System.Windows.Forms;
using NPacMan.SharedUi;
using NPacMan.UI.Bots;

namespace NPacMan.UI
{
    public partial class Form1 : Form
    {
        private readonly Timer _renderLoop = new Timer();
        private readonly BoardRenderer _boardRenderer = new BoardRenderer();
        private readonly GraphicsBuffers _graphicsBuffers;
        private readonly Game.Game _game;
        public Form1(string[] args)
        {
            InitializeComponent();

            _game = Game.Game.Create()
                             .AddKeyboard()
                            // .AddSounds()
                             .AddBots(args)
                             .StartGame();

            _graphicsBuffers = new GraphicsBuffers(this) { ShowFps = true };

            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "PacMan";
            this.WindowState = FormWindowState.Maximized;

            _renderLoop.Interval = 16; //40fps
            _renderLoop.Enabled = true;
            _renderLoop.Tick += _renderLoop_Tick;
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
