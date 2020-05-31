using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PacManDebugger
{
    public partial class Form1 : Form
    {

        private readonly Button cmdStart;
        private readonly Button cmdDisplay;
        private readonly HScrollBar tickSelector;
        private readonly History _history = new History();
        private readonly Label lblTick;
        private int _y = 100;

        public Form1()
        {
            cmdStart = new Button
            {
                AutoSize = true,
                Location = new System.Drawing.Point(74, 30),
                Name = "cmdStart",
                Size = new System.Drawing.Size(41, 25),
                TabIndex = 0,
                Text = "Start"
            };
            cmdStart.Click += cmdStart_Click;

            cmdDisplay = new Button
            {
                AutoSize = true,
                Location = new System.Drawing.Point(185, 30),
                Name = "cmdCount",
                Size = new System.Drawing.Size(38, 25),
                TabIndex = 1,
                Text = "Display"
            };
            cmdDisplay.Click += cmdDisplay_Click;

            tickSelector = new HScrollBar
            {
                Location = new System.Drawing.Point(10, 700),
                Width = 400,
                Minimum = 0,
                Maximum = 1000,
                Value = 10,
            };
            tickSelector.ValueChanged += TickSelector_ValueChanged;

            lblTick = new Label
            {
                Location = new System.Drawing.Point(tickSelector.Left + tickSelector.Width + 20, tickSelector.Top),
                AutoSize = true,
                Text = "Tick: "
            };
            this.Controls.Add(lblTick);

            ClientSize = new System.Drawing.Size(697, 800);
            Controls.Add(cmdDisplay);
            Controls.Add(cmdStart);
            Controls.Add(tickSelector);
        }

        private void TickSelector_ValueChanged(object? sender, EventArgs e)
        {
            var tickCount = tickSelector.Value;

            lblTick.Text = $"Tick: {tickCount}";

            DisplayGhostsAtTime(tickCount);
        }

        private void cmdStart_Click(object? sender, EventArgs e)
        {
            Task.Run(() =>
           {
               var session = new TraceEventSession("TestEvents");

               session.EnableProvider("PacManEventSource", Microsoft.Diagnostics.Tracing.TraceEventLevel.Always);

               session.Source.Dynamic.AddCallbackForProviderEvent("PacManEventSource", "GhostMoved", traceEvent =>
               {
                   var ghostName = (string)traceEvent.PayloadByName("ghostName");
                   var tickCounter = (int)traceEvent.PayloadByName("tickCounter");
                   var fromLocationX = (int)traceEvent.PayloadByName("fromLocationX");
                   var fromLocationY = (int)traceEvent.PayloadByName("fromLocationY");
                   var toLocationX = (int)traceEvent.PayloadByName("toLocationX");
                   var toLocationY = (int)traceEvent.PayloadByName("toLocationY");

                   _history.AddHistoricEvent(ghostName, tickCounter, new Location(fromLocationX, fromLocationY), new Location(toLocationX, toLocationY));
               });

               session.Source.Process();
           });
        }

        private void cmdDisplay_Click(object? sender, EventArgs e)
        {
            var tickCount = tickSelector.Value;

            DisplayGhostsAtTime(tickCount);
        }

        private void DisplayGhostsAtTime(int tickCount)
        {
            var ghosts = _history.GhostNames();

            var g = this.CreateGraphics();
            g.TranslateTransform(50, 100);
            const int rowHeight = 10;
            const int columnWidth = 10;

            DrawGrid(g, rowHeight, columnWidth);

            foreach (var ghostName in ghosts)
            {
                var lbl = FindLabelForGhost(ghostName);
                if (lbl is null)
                {
                    lbl = new Label();
                    lbl.Location = new System.Drawing.Point(1000, _y);
                    lbl.AutoSize = true;
                    lbl.Tag = ghostName;
                    _y += (int)(lbl.Height * 1.5);
                    this.Controls.Add(lbl);
                }

                var eventDetails = _history.GetHistoricEventForTickCount(ghostName, tickCount);
                lbl.Text = $"{ghostName} went from {eventDetails.OriginalLocation} to {eventDetails.FinalLocation}";

                var x = eventDetails.FinalLocation.X * columnWidth;
                var y = eventDetails.FinalLocation.Y * rowHeight;
                g.FillEllipse(Brushes.Red, x, y, columnWidth, rowHeight);
            }
        }

        private static void DrawGrid(Graphics g, int rowHeight, int columnWidth)
        {
            for (int rowNumber = 0; rowNumber <= 50; rowNumber++)
            {
                g.DrawLine(Pens.Blue, 0, rowNumber * rowHeight, columnWidth * 50, rowNumber * rowHeight);
            }

            for (int columnNumber = 0; columnNumber <= 50; columnNumber++)
            {
                g.DrawLine(Pens.Blue, columnNumber * columnWidth, 0, columnNumber * columnWidth, 50 * rowHeight);
            }
        }

        private Label? FindLabelForGhost(string ghostName)
        {
            foreach (var ctl in this.Controls)
            {
                if (ctl is Label lbl && (lbl.Tag is string) && lbl.Tag.Equals(ghostName))
                    return lbl;
            }

            return null;
        }
    }

}
