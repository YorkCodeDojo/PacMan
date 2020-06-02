using System.Drawing;
using System.Windows.Forms;

namespace PacManDebugger
{
    public partial class PacManDetails : UserControl
    {
        private readonly Label lblPreviousLocation;
        private readonly Label lblCurrentLocation;
        private readonly CheckBox cbDisplayPacman;

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
                TabStop = false
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
                Location = new Point(95, 60)
            };
            Controls.Add(cbDisplayPacman);
        }

        internal void ShowDetails(HistoricEvent eventDetails)
        {
            lblPreviousLocation.Text = $"Previous Location : { eventDetails.OriginalLocation}";
            lblCurrentLocation.Text = $"Current Location : { eventDetails.FinalLocation}";
        }
    }
}
