using NPacMan.Game;
using System;
using System.Runtime.InteropServices;
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

        [Flags]
        private enum KeyStates
        {
            None = 0,
            Down = 1,
            Toggled = 2
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);

        private static KeyStates GetKeyState(Keys key)
        {
            KeyStates state = KeyStates.None;

            short retVal = GetKeyState((int)key);

            //If the high-order bit is 1, the key is down
            //otherwise, it is up.
            if ((retVal & 0x8000) == 0x8000)
                state |= KeyStates.Down;

            //If the low-order bit is 1, the key is toggled.
            if ((retVal & 1) == 1)
                state |= KeyStates.Toggled;

            return state;
        }

        public static bool IsKeyDown(Keys key)
        {
            return KeyStates.Down == (GetKeyState(key) & KeyStates.Down);
        }


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
                             .Subscribe(GameNotification.PreTick, CheckForKeyPress)
                             .StartGame();

            _graphicsBuffers = new GraphicsBuffers(this) { ShowFps = true };

            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "PacMan";
            this.WindowState = FormWindowState.Maximized;

            _renderLoop.Interval = 16; //40fps
            _renderLoop.Enabled = true;
            _renderLoop.Tick += _renderLoop_Tick;
        }

        private async void CheckForKeyPress()
        {
            if (IsKeyDown(Keys.Left)) 
                await _game.ChangeDirection(Direction.Left);
            else if (IsKeyDown(Keys.Right))
                await _game.ChangeDirection(Direction.Right);
            else if (IsKeyDown(Keys.Up))
                await _game.ChangeDirection(Direction.Up);
            else if (IsKeyDown(Keys.Down))
                await _game.ChangeDirection(Direction.Down);
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
