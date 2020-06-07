using System;
using System.Windows.Forms;
using NPacMan.Game;
using NPacMan.LevelDesigner;
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

            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "PacMan Designer";
            this.WindowState = FormWindowState.Maximized;

            this.Click += Form1_Click;
        }

        private void Form1_Click(object? sender, EventArgs e)
        {
            var pixelX = MousePosition.X;
            var pixelY = MousePosition.Y;

            var graphicsBuffers = new GraphicsBuffers(this) { ShowFps = false };
            graphicsBuffers.CalculateOffsets(_currentDesign.Width, _currentDesign.Height + 5);

            var x = (int)((pixelX - graphicsBuffers.OffsetX) / 8)-2;
            var y = (int)((pixelY - graphicsBuffers.OffsetY) / 8)-3-12;

            MessageBox.Show($"{x}, {y}");

            try
            {
                if (_addCoin.Checked)
                {
                    _currentDesign.AddCoin(x, y);
                }
                else if (_addWall.Checked)
                {
                    _currentDesign.AddWall(x, y);
                }
                else if (_addPill.Checked)
                {
                    _currentDesign.AddPowerPill(x, y);
                }

                DisplayBoard();
            }
            catch (Exception)
            {
            }


        }

        private void DisplayBoard()
        {
            var game = new Game.Game(new GameClock(), _currentDesign.GameSettingsForDesign());

            var boardRenderer = new BoardRenderer();
            boardRenderer.RenderStart(game);

            var graphicsBuffers = new GraphicsBuffers(this) { ShowFps = false };
            graphicsBuffers.RenderBackgroundUpdate(boardRenderer);
            graphicsBuffers.RenderBackgroundUpdate(boardRenderer);
        }
    }
}
