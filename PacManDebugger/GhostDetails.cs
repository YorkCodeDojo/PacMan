using System.Drawing;
using System.Windows.Forms;

namespace PacManDebugger
{
    public partial class GhostDetails : UserControl
    {
        private readonly Label lblPreviousLocation;
        private readonly Label lblCurrentLocation;
        private readonly Label lblStrategy;
        private readonly Label lblEdible;
        private readonly CheckBox cbDisplayGhost;

        public string GhostName { get; }

        public GhostDetails(string ghostName)
        {
            GhostName = ghostName;

            InitializeComponent();

            var image = ghostName switch
            {
                GhostNames.Blinky => Image.FromFile("images/blinky.JPG"),
                GhostNames.Clyde => Image.FromFile("images/clyde.JPG"),
                GhostNames.Inky => Image.FromFile("images/inky.JPG"),
                GhostNames.Pinky => Image.FromFile("images/pinky.JPG"),
                _ => Image.FromFile("images/unknown.JPG"),
            };

            var pbGhost = new PictureBox
            {
                Image = image,
                Location = new Point(0, 0),
                Name = "pbGhost",
                Size = new Size(82, 115),
                TabIndex = 4,
                TabStop = false
            };
            Controls.Add(pbGhost);

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

            lblStrategy = new Label
            {
                Text = "Strategy",
                AutoSize = true,
                Location = new Point(230, 20)
            };
            Controls.Add(lblStrategy);

            lblEdible = new Label
            {
                Text = "Edible",
                AutoSize = true,
                Location = new Point(230, 40)
            };
            Controls.Add(lblEdible);

            cbDisplayGhost = new CheckBox
            {
                Text = "Display Ghost",
                AutoSize = true,
                Location = new Point(95, 60)
            };
            Controls.Add(cbDisplayGhost);
        }

        internal void ShowDetails(HistoricEvent eventDetails)
        {
            lblPreviousLocation.Text = $"Previous Location : { eventDetails.OriginalLocation}";
            lblCurrentLocation.Text = $"Current Location : { eventDetails.FinalLocation}";
            lblStrategy.Text = "Strategy : Chase";
            lblEdible.Text = "Edible : Yes";
        }
    }
}
