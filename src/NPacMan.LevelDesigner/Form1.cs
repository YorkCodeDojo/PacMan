using System;
using System.Reflection;
using System.Windows.Forms;
using NPacMan.Game;
using NPacMan.SharedUi;

namespace NPacMan.UI
{
    public partial class Form1 : Form
    {
        private readonly CurrentDesign _currentDesign = new CurrentDesign();
        private readonly RadioButton _addWall;
        private readonly RadioButton _addCoin;
        private readonly RadioButton _addPill;

        public Form1()
        {
            InitializeComponent();

            var lbl = new Label
            {
                Text = "Toolbox",
                Location = new System.Drawing.Point(10, 5)
            };
            this.Controls.Add(lbl);

            _addWall = new RadioButton
            {
                Name = "rbWall",
                Text = "Add Wall",
                Location = new System.Drawing.Point(10, 30)
            };
            this.Controls.Add(_addWall);

            _addCoin = new RadioButton
            {
                Name = "rbCoin",
                Text = "Add Coin",
                Location = new System.Drawing.Point(10, 60)
            };
            this.Controls.Add(_addCoin);

            _addPill = new RadioButton
            {
                Name = "rbPill",
                Text = "Add Pill",
                Location = new System.Drawing.Point(10, 90)
            };
            this.Controls.Add(_addPill);

            var cmd = new Button
            {
                Text = "Click",
                Location = new System.Drawing.Point(200, 0)
            };
            this.Controls.Add(cmd);

            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "PacMan Designer";
            this.WindowState = FormWindowState.Maximized;

            _currentDesign.InitialLives = 0;
            _currentDesign.Height = 30;
            _currentDesign.Width = 30;

            _currentDesign.Walls.Add(new CellLocation(0, 0));
            _currentDesign.Walls.Add(new CellLocation(1, 0));
            _currentDesign.Walls.Add(new CellLocation(2, 0));
            _currentDesign.Walls.Add(new CellLocation(3, 0));
            _currentDesign.Walls.Add(new CellLocation(0, 1));
            _currentDesign.Walls.Add(new CellLocation(0, 2));
            _currentDesign.Walls.Add(new CellLocation(0, 3));

            this.Click += Form1_Click;
        }

        private void Form1_Click(object? sender, EventArgs e)
        {
            var pixelX = MousePosition.X;
            var pixelY = MousePosition.Y;

            var graphicsBuffers = new GraphicsBuffers(this) { ShowFps = false };
            graphicsBuffers.CalculateOffsets(_currentDesign.Width, _currentDesign.Height + 5);

            var x = (int)((pixelX - graphicsBuffers.OffsetX) / 8);
            var y = (int)((pixelY - graphicsBuffers.OffsetY) / 8);


            MessageBox.Show($"{x}, {y}");

            if (_addCoin.Checked)
            {
                _currentDesign.Coins.Add(new CellLocation(x, y));
            }

            if (_addWall.Checked)
            {
                _currentDesign.Walls.Add(new CellLocation(x, y));
            }

            if (_addPill.Checked)
            {
                _currentDesign.PowerPills.Add(new CellLocation(x, y));
            }

            DisplayBoard();
        }

        private void DisplayBoard()
        {
            var game = new Game.Game(new GameClock(), _currentDesign);

            var boardRenderer = new BoardRenderer();
            boardRenderer.RenderStart(game);

            var graphicsBuffers = new GraphicsBuffers(this) { ShowFps = false };
            graphicsBuffers.RenderBackgroundUpdate(boardRenderer);
            graphicsBuffers.RenderBackgroundUpdate(boardRenderer);
        }
    }
}
