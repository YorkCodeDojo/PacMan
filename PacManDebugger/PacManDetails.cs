using System;
using System.Drawing;
using System.Windows.Forms;

namespace PacManDebugger
{
    public partial class PacManDetails : UserControl
    {
        private readonly Label lblPreviousLocation;
        private readonly Label lblCurrentLocation;
        private readonly CheckBox cbDisplayPacman;
        private Label lblLives;
        private readonly Label lblDirection;
        private readonly Label lblScore;

        public event EventHandler? DisplayPacmanChanged;

        public bool DisplayOnMap => cbDisplayPacman.Checked;

        public PacManDetails()
        {
            InitializeComponent();

            var pbPacMan = new PictureBox
            {
                Image = Image.FromFile("images/pacman.JPG"),
                Location = new Point(0, 0),
                Name = "pbPacMan",
                Size = new Size(82, 115),
                TabIndex = 4,
                TabStop = false,
                SizeMode = PictureBoxSizeMode.StretchImage,
            };
            Controls.Add(pbPacMan);

            lblPreviousLocation = new Label
            {
                Text = "Previous Location",
                AutoSize = true,
                Location = new Point(95, 20)
            };
            Controls.Add(lblPreviousLocation);

            lblCurrentLocation = new Label
            {
                Text = "Current Location",
                AutoSize = true,
                Location = new Point(95, 40)
            };
            Controls.Add(lblCurrentLocation);

            cbDisplayPacman = new CheckBox
            {
                Text = "Display PacMan",
                AutoSize = true,
                Location = new Point(95, 60),
                Checked = true,
            };

            lblLives = new Label
            {
                Text = "Lives",
                AutoSize = true,
                Location = new Point(260, 20)
            };
            Controls.Add(lblLives);

            lblScore = new Label
            {
                Text = "Score",
                AutoSize = true,
                Location = new Point(260, 40)
            };
            Controls.Add(lblScore);

            lblDirection = new Label
            {
                Text = "Direction",
                AutoSize = true,
                Location = new Point(260, 60)
            };
            Controls.Add(lblDirection);


            cbDisplayPacman.CheckedChanged += CbDisplayPacman_CheckedChanged;
            Controls.Add(cbDisplayPacman);
        }

        private void CbDisplayPacman_CheckedChanged(object? sender, EventArgs e)
        {
            DisplayPacmanChanged?.Invoke(this, EventArgs.Empty);
        }

        internal void ShowDetails(HistoricPacManMovementEvent eventDetails, HistoricPacManStateChangeEvent stateDetails)
        {
            lblPreviousLocation.Text = $"Previous Location : { eventDetails.OriginalLocation}";
            lblCurrentLocation.Text = $"Current Location : { eventDetails.FinalLocation}";
            lblScore.Text = $"Lives : { stateDetails.Lives}";
            lblLives.Text = $"Score : { stateDetails.Score}";
            lblDirection.Text = $"Facing : { stateDetails.Direction}";
        }
    }
}
