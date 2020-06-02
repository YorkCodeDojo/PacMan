using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PacManDebugger
{
    public partial class Form1 : Form
    {
        private readonly History _history = new History();
        private readonly Board _board = new Board();
        private readonly HScrollBar tickSelector;
        private readonly Label lblTick;
        private readonly EventConsumer _eventConsumer;
        private int _y = 100;

        public Form1()
        {
            _eventConsumer = new EventConsumer(_history, _board);

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));

            tickSelector = new HScrollBar
            {
                Location = new System.Drawing.Point(10, 60),
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

            var toolStripButtonStart = new ToolStripButton
            {
                //    toolStripButton1.Image = (Image)resources.GetObject("pacman")!;
                ImageTransparentColor = System.Drawing.Color.Magenta,
                Name = "toolStripButtonStart",
                Size = new System.Drawing.Size(96, 22),
                Text = "Start Capture",
                DisplayStyle = ToolStripItemDisplayStyle.Text
            };
            toolStripButtonStart.Click += ToolStripButtonStart_Click;

            var toolStripButtonStop = new ToolStripButton
            {
                //    toolStripButton1.Image = (Image)resources.GetObject("pacman")!;
                ImageTransparentColor = System.Drawing.Color.Magenta,
                Name = "toolStripButtonStop",
                Size = new System.Drawing.Size(96, 22),
                Text = "Stop Capture",
                DisplayStyle = ToolStripItemDisplayStyle.Text
            };
            toolStripButtonStop.Click += ToolStripButtonStop_Click;

            var toolStripButtonClear = new ToolStripButton
            {
                //    toolStripButton1.Image = (Image)resources.GetObject("pacman")!;
                ImageTransparentColor = System.Drawing.Color.Magenta,
                Name = "toolStripButtonClear",
                Size = new System.Drawing.Size(96, 22),
                Text = "Clear Capture",
                DisplayStyle = ToolStripItemDisplayStyle.Text
            };
            toolStripButtonClear.Click += ToolStripButtonClear_Click;


            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButtonStart, toolStripButtonStop, toolStripButtonClear });
            this.Controls.Add(toolStrip1);

            ClientSize = new System.Drawing.Size(697, 800);
            Controls.Add(tickSelector);

            this.Text = "PacMan Debugger";
            this.BackColor = Color.White;
            this.Icon = (Icon)resources.GetObject("pacman")!;

        }

        private void ToolStripButtonStart_Click(object? sender, EventArgs e)
        {
            _eventConsumer.Start();
        }

        private void ToolStripButtonStop_Click(object? sender, EventArgs e)
        {
            _eventConsumer.Stop();
        }

        private void ToolStripButtonClear_Click(object? sender, EventArgs e)
        {
            _board.Clear();
            _history.Clear();
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
            const int rowHeight = 15;
            const int columnWidth = 15;

            var ghosts = _history.GhostNames();

            var currentContext = BufferedGraphicsManager.Current;
            var grid = new Rectangle(50, 100, columnWidth * 50, 50 * rowHeight);
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

                var x = (2 + eventDetails.FinalLocation.X) * columnWidth;
                var y = (2 + eventDetails.FinalLocation.Y) * rowHeight;
                g.FillEllipse(Brushes.Red, x, y, columnWidth, rowHeight);
            }

            myBuffer.Render();
        }

        private void DrawGrid(Graphics g, int rowHeight, int columnWidth)
        {
            g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, columnWidth * 50, _board.Height * 50);

            g.FillRectangle(Brushes.Black, 0, 0, columnWidth * (_board.Width + 4), (_board.Height + 4) * rowHeight);

            for (int rowNumber = 0; rowNumber <= _board.Height + 4; rowNumber++)
            {
                g.DrawLine(Pens.DimGray, 0, rowNumber * rowHeight, columnWidth * (_board.Width + 4), rowNumber * rowHeight);
            }

            for (int columnNumber = 0; columnNumber <= _board.Width + 4; columnNumber++)
            {
                g.DrawLine(Pens.DimGray, columnNumber * columnWidth, 0, columnNumber * columnWidth, (_board.Height + 4) * rowHeight);
            }

            for (int rowNumber = 0; rowNumber <= _board.Height; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber <= _board.Width; columnNumber++)
                {
                    if (_board.Walls.Any(w => w.X == columnNumber && w.Y == rowNumber))
                    {
                        g.FillRectangle(Brushes.LightBlue, ((2 + columnNumber) * columnWidth) + 1, ((2 + rowNumber) * rowHeight) + 1, columnWidth - 2, rowHeight - 2);
                    }
                }
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
