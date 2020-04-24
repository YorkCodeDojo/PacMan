using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NPacMan.Game;

namespace NPacMan.UI
{
    public partial class Form1 : Form
    {
        private readonly Timer _renderLoop = new Timer();
        private readonly BoardRenderer _boardRenderer = new BoardRenderer();
        private readonly Game.Game _game = Game.Game.Create();

        public Form1()
        {
            InitializeComponent();

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
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(_keysMap.TryGetValue(e.KeyCode, out var direction))
            {
                _game.ChangeDirection(direction);
            }
        }

        private void _renderLoop_Tick(object sender, EventArgs e)
        {
            try
            {
                var currentContext = BufferedGraphicsManager.Current;
                using var myBuffer = currentContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);
                var g = myBuffer.Graphics;

                _boardRenderer.RenderWalls(g, this.ClientSize.Width, this.ClientSize.Height, _game);
                _boardRenderer.RenderCoins(g, this.ClientSize.Width, this.ClientSize.Height, _game);
                _boardRenderer.RenderPacMan(g, this.ClientSize.Width, this.ClientSize.Height, _game);
                _boardRenderer.RenderScore(g, this.ClientSize.Width, this.ClientSize.Height, _game);
                _boardRenderer.RenderGhosts(g, this.ClientSize.Width, this.ClientSize.Height, _game);

                myBuffer.Render();
            }
            catch (System.ObjectDisposedException)
            {
                // App closing - this.CreateGraphics() will fail
            }

        }
    }
}
