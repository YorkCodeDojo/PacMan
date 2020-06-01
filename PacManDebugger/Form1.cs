using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PacManDebugger
{
    public partial class Form1 : Form
    {
        private readonly HScrollBar tickSelector;
        private readonly History _history = new History();
        private readonly Label lblTick;
        private int _y = 100;

        public Form1()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));

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

            var toolStrip1 = new System.Windows.Forms.ToolStrip
            {
                Location = new System.Drawing.Point(0, 0),
                Name = "toolStrip1",
                Size = new System.Drawing.Size(800, 25),
                TabIndex = 0,
                Text = "toolStrip1"
            };

            var toolStripButtonStart = new System.Windows.Forms.ToolStripButton
            {
                //    toolStripButton1.Image = (Image)resources.GetObject("pacman")!;
                ImageTransparentColor = System.Drawing.Color.Magenta,
                Name = "toolStripButtonStart",
                Size = new System.Drawing.Size(96, 22),
                Text = "Start Capture",
                DisplayStyle = ToolStripItemDisplayStyle.Text
            };
            toolStripButtonStart.Click += ToolStripButtonStart_Click;
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripButtonStart });
            this.Controls.Add(toolStrip1);

            ClientSize = new System.Drawing.Size(697, 800);
            Controls.Add(tickSelector);

            this.Text = "PacMan Debugger";
            this.BackColor = Color.White;
            this.Icon = (Icon)resources.GetObject("pacman")!;

        }

        private void ToolStripButtonStart_Click(object? sender, EventArgs e)
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

        private void TickSelector_ValueChanged(object? sender, EventArgs e)
        {
            var tickCount = tickSelector.Value;

            lblTick.Text = $"Tick: {tickCount}";

            DisplayGhostsAtTime(tickCount);
        }

        private void cmdDisplay_Click(object? sender, EventArgs e)
        {
            var tickCount = tickSelector.Value;

            DisplayGhostsAtTime(tickCount);
        }

        private void DisplayGhostsAtTime(int tickCount)
        {
            const int rowHeight = 10;
            const int columnWidth = 10;

            var ghosts = _history.GhostNames();

            var currentContext = BufferedGraphicsManager.Current;
            var grid = new Rectangle(50,100, columnWidth * 50, 50 * rowHeight);
            using var myBuffer = currentContext.Allocate(this.CreateGraphics(), grid);
            var g = myBuffer.Graphics;

            g.TranslateTransform(50, 100);

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

            myBuffer.Render();
        }

        private static void DrawGrid(Graphics g, int rowHeight, int columnWidth)
        {
            g.FillRectangle(Brushes.Black, 0, 0, columnWidth * 50, 50 * rowHeight);

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
