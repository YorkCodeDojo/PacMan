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

        private void _renderLoop_Tick(object? sender, EventArgs e)
        {
            try
            {
                var currentContext = BufferedGraphicsManager.Current;
                using var screenBuffer = currentContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);

                using var gameBuffer = new Bitmap(_game.Width * Sprites.PixelGrid, (_game.Height + 5) * Sprites.PixelGrid);
                {
                    var g = Graphics.FromImage(gameBuffer);
                    
                    _boardRenderer.RenderScore(g, _game);

                    g.TranslateTransform(0, 3 * Sprites.PixelGrid);
                    _boardRenderer.RenderWalls(g, _game);
                    _boardRenderer.RenderCoins(g, _game);
                    _boardRenderer.RenderPacMan(g, _game);
                    _boardRenderer.RenderGhosts(g, _game);
                    g.ResetTransform();

                    g.TranslateTransform(0, (3 + _game.Height) * Sprites.PixelGrid);
                    _boardRenderer.RenderLives(g, _game);
                    g.ResetTransform();
                }
                {
                    var g = screenBuffer.Graphics;
                    var scale = Math.Min((float)ClientSize.Width / gameBuffer.Width, (float)ClientSize.Height / gameBuffer.Height);
                    g.DrawImage(gameBuffer, new RectangleF(0f, 0f, gameBuffer.Width * scale, gameBuffer.Height * scale), 
                        new RectangleF(0,0,gameBuffer.Width, gameBuffer.Height), 
                        GraphicsUnit.Pixel);
                }

                var gScreen = screenBuffer.Graphics;

                screenBuffer.Render();
            }
            catch (System.ObjectDisposedException)
            {
                // App closing - this.CreateGraphics() will fail
            }

        }
    }
}
