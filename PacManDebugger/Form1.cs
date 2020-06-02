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
        private readonly GroupBox gbTime;
        private readonly Label lblStart;
        private readonly Label lblEnd;
        private readonly EventConsumer _eventConsumer;
        private PacManDetails? pacManDetails;
        private int _y = 100;

        public Form1()
        {
            _eventConsumer = new EventConsumer(_history, _board);

            lblStart = new Label
            {
                AutoSize = true,
                Location = new Point(15, 76),
                Name = "lblStart",
                Size = new Size(19, 21),
                TabIndex = 2,
                Text = "0"
            };

            lblEnd = new Label
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                AutoSize = true,
                Location = new Point(1072, 73),
                Name = "lblEnd",
                Size = new Size(46, 21),
                TabIndex = 2,
                Text = "1000"
            };

            tickSelector = new HScrollBar
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(15, 33),
                Name = "tickSelector",
                Size = new Size(1098, 32),
                TabIndex = 1,
                Minimum = 0,
                Maximum = 1000,
                Value = 10,
            };
            tickSelector.ValueChanged += TickSelector_ValueChanged;

            var resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));

            gbTime = new GroupBox
            {
                Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right,
                Location = new Point(12, 37),
                Name = "groupBox1",
                Size = new Size(1132, 113),
                TabIndex = 3,
                TabStop = false,
                Text = "Displaying Tick Number:"
            };
            gbTime.Controls.Add(tickSelector);
            gbTime.Controls.Add(lblStart);
            gbTime.Controls.Add(lblEnd);

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

            Location = new Point(100, 100);
            ClientSize = new Size(1156, 1000);

            this.Controls.Add(gbTime);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            this.Text = "PacMan Debugger - Not Started";
            this.BackColor = Color.White;
            this.Icon = (Icon)resources.GetObject("pacman")!;

            _y = gbTime.Top + gbTime.Height + 30;
        }

        private void ToolStripButtonStart_Click(object? sender, EventArgs e)
        {
            _eventConsumer.Start();
            this.Text = "PacMan Debugger - Capturing";
        }

        private void ToolStripButtonStop_Click(object? sender, EventArgs e)
        {
            _eventConsumer.Stop();
            this.Text = "PacMan Debugger - Stopped";
        }

        private void ToolStripButtonClear_Click(object? sender, EventArgs e)
        {
            _board.Clear();
            _history.Clear();
        }

        private void TickSelector_ValueChanged(object? sender, EventArgs e)
        {
            var tickCount = tickSelector.Value;

            gbTime.Text = $"Displaying Tick Number: {tickCount}";

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

            const int rowsToDisplay = 50;
            const int columnsToDisplay = 50;

            var gridLeft = gbTime.Left;
            var gridTop = gbTime.Top + gbTime.Height + 30;
            var gridWidth = columnWidth * columnsToDisplay;
            var gridRight = gridLeft + gridWidth;

            var xOffset = (columnsToDisplay - _board.Width) / 2;
            var yOffset = (rowsToDisplay - _board.Height) / 2;

            var currentContext = BufferedGraphicsManager.Current;
            var grid = new Rectangle(gridLeft, gridTop, gridWidth, rowsToDisplay * rowHeight);
            using var myBuffer = currentContext.Allocate(this.CreateGraphics(), grid);
            var g = myBuffer.Graphics;

            g.TranslateTransform(gridLeft, gridTop);

            DrawGrid(g, rowHeight, columnWidth, xOffset, yOffset, columnsToDisplay, rowsToDisplay);

            DisplayPacMan(gridRight);

            var ghosts = _history.GhostNames();
            foreach (var ghostName in ghosts)
            {
                DisplayGhost(tickCount, rowHeight, columnWidth, g, ghostName, xOffset, yOffset, gridRight);
            }

            myBuffer.Render();
        }

        private void DisplayPacMan(int gridRight)
        {
            if (pacManDetails is null)
            {
                pacManDetails = new PacManDetails()
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    Location = new Point(gridRight + 30, _y),
                    Size = new Size(583, 116),
                };
                Controls.Add(pacManDetails);

                _y += (int)(pacManDetails.Height * 1.2);
            }
        }

        private void DisplayGhost(int tickCount, int rowHeight, int columnWidth, Graphics g, string ghostName, int xOffset, int yOffset, int gridRight)
        {
            var ghostColour = ghostName switch
            {
                GhostNames.Blinky => new SolidBrush(Color.FromArgb(245, 41, 0)),
                GhostNames.Clyde => new SolidBrush(Color.FromArgb(255, 160, 6)),
                GhostNames.Inky => new SolidBrush(Color.FromArgb(6, 219, 223)),
                GhostNames.Pinky => new SolidBrush(Color.FromArgb(255, 176, 180)),
                _ => Brushes.White,
            };

            var ghostDetails = FindControlForGhost(ghostName);
            if (ghostDetails is null)
            {
                ghostDetails = new GhostDetails(ghostName)
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    Location = new Point(gridRight + 30, _y),
                    Size = new Size(583, 116),
                };
                Controls.Add(ghostDetails);

                _y += (int)(ghostDetails.Height * 1.2);
            }

            var eventDetails = _history.GetHistoricEventForTickCount(ghostName, tickCount);
            ghostDetails.ShowDetails(eventDetails);

            var x = (xOffset + eventDetails.FinalLocation.X) * columnWidth;
            var y = (yOffset + eventDetails.FinalLocation.Y) * rowHeight;
            g.FillEllipse(ghostColour, x, y, columnWidth, rowHeight);

        }

        private void DrawGrid(Graphics g, int rowHeight, int columnWidth, int xOffset, int yOffset, int columnsToDisplay, int rowsToDisplay)
        {
            g.FillRectangle(Brushes.Black, 0, 0, columnWidth * columnsToDisplay, rowHeight * rowsToDisplay);

            for (int rowNumber = 0; rowNumber <= rowsToDisplay; rowNumber++)
            {
                g.DrawLine(Pens.DimGray, 0, rowNumber * rowHeight, columnWidth * columnsToDisplay, rowNumber * rowHeight);
            }

            for (int columnNumber = 0; columnNumber <= columnsToDisplay; columnNumber++)
            {
                g.DrawLine(Pens.DimGray, columnNumber * columnWidth, 0, columnNumber * columnWidth, rowsToDisplay * rowHeight);
            }

            for (int rowNumber = 0; rowNumber <= _board.Height; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber <= _board.Width; columnNumber++)
                {
                    if (_board.Walls.Any(w => w.X == columnNumber && w.Y == rowNumber))
                    {
                        g.FillRectangle(Brushes.LightBlue, ((xOffset + columnNumber) * columnWidth) + 1, ((yOffset + rowNumber) * rowHeight) + 1, columnWidth - 2, rowHeight - 2);
                    }
                }
            }
        }

        private GhostDetails? FindControlForGhost(string ghostName)
        {
            foreach (var ctl in this.Controls)
            {
                if (ctl is GhostDetails gd && gd.GhostName.Equals(ghostName))
                    return gd;
            }

            return null;
        }
    }

}
