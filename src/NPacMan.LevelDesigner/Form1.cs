using NPacMan.Game;
using NPacMan.LevelDesigner;
using NPacMan.SharedUi;
using System;
using System.Windows.Forms;

namespace NPacMan.UI
{
    public partial class Form1 : Form
    {
        private readonly CurrentDesign _currentDesign = new CurrentDesign();
        private readonly RadioButton _addWall;
        private readonly RadioButton _addCoin;
        private readonly RadioButton _addPill;
        private readonly RadioButton _addPacManLeft;
        private readonly RadioButton _addPacManRight;
        private readonly RadioButton _addPacManUp;
        private readonly RadioButton _addPacManDown;
        private readonly RadioButton _addDoor;
        private readonly RadioButton _addPortal;
        private readonly RadioButton _addGhostHouse;
        private readonly RadioButton _addFruit;

        public Form1()
        {
            InitializeComponent();

            var lbl = new Label
            {
                Text = "Toolbox",
                Location = new System.Drawing.Point(10, 5)
            };
            this.Controls.Add(lbl);

            _addWall = AddRadioButton("rbWall", "Add Wall", 1);
            _addCoin = AddRadioButton("rbCoin", "Add Coin", 2);
            _addPill = AddRadioButton("rbPill", "Add Pill", 3);
            _addPacManLeft = AddRadioButton("addPacManLeft", "Add PacMan Left", 4);
            _addPacManRight = AddRadioButton("addPacManLeft", "Add PacMan Right", 5);
            _addPacManUp = AddRadioButton("addPacManLeft", "Add PacMan Up", 6);
            _addPacManDown = AddRadioButton("addPacManLeft", "Add PacMan Down", 7);
            _addDoor = AddRadioButton("AddDoor", "Add PacMan Down", 8);
            _addPortal = AddRadioButton("AddPortal", "Add Portal", 9);
            _addGhostHouse = AddRadioButton("AddGhostHouse", "Add Ghost House", 10);
            _addFruit = AddRadioButton("AddFruit", "Add Fruit", 11);

            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "PacMan Designer";
            this.WindowState = FormWindowState.Maximized;

            this.Click += Form1_Click;
        }

        private RadioButton AddRadioButton(string name, string text, int buttonNumber)
        {
            var button = new RadioButton
            {
                Name = name,
                Text = text,
                Location = new System.Drawing.Point(10, buttonNumber * 30),
                AutoSize = true,
            };
            this.Controls.Add(button);

            return button;
        }

        private void Form1_Click(object? sender, EventArgs e)
        {
            var pixelX = MousePosition.X;
            var pixelY = MousePosition.Y;

            var graphicsBuffers = new GraphicsBuffers(this) { ShowFps = false };
            graphicsBuffers.CalculateOffsets(_currentDesign.Width, _currentDesign.Height + 5);

            var x = (int)((pixelX - graphicsBuffers.OffsetX) / 8) - 2;
            var y = (int)((pixelY - graphicsBuffers.OffsetY) / 8) - 3 - 12;

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

        private void Form1_Load(object sender, EventArgs e)
        {
            DisplayBoard();
        }
    }
}
