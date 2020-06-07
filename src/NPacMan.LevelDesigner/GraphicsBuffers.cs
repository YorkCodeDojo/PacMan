using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using NPacMan.SharedUi;
using NPacMan.SharedUi.Properties;

namespace NPacMan.UI
{
    public class GraphicsBuffers : IDisposable
    {
        private BufferedGraphicsContext _currentContext;

        private BufferedGraphics? _screenBuffer;
        private Graphics? _screenGraphics;

        private Bitmap? _gameBuffer;
        private Graphics? _gameGraphics;

        private Bitmap? _backgroundBuffer;
        private Graphics? _backgroundGraphics;

        private readonly Form _form;

        private Rectangle _formSize;

        private Rectangle SizeFromForm => _form.ClientRectangle;

        public bool ShowFps = false;

        private readonly Stopwatch _fpsStopWatch;
        private int _fpsCounter = 0;
        private int _fpsLast = 0;

        private readonly Font _fpsFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
        private readonly Brush _fpsBrush = new SolidBrush(Color.DarkRed);

        private Bitmap Gfx;

        public float OffsetX { get; private set; }
        public float OffsetY { get; private set; }


        public GraphicsBuffers(Form forms)
        {
            _form = forms;
            _currentContext = BufferedGraphicsManager.Current;
            _formSize = _form.DisplayRectangle;

            _fpsStopWatch = new Stopwatch();
            _fpsStopWatch.Start();

            using var ms = new MemoryStream(Resources.gfx);
            Gfx = new Bitmap(ms);
        }

        /// <summary>
        /// Create a new form buffer if we haven't got one, or the size has changed
        /// </summary>
        private void GetFormBuffer()
        {
            if (_screenGraphics == null || _screenBuffer == null || _formSize != SizeFromForm)
            {
                _formSize = _form.DisplayRectangle;

                _screenGraphics?.Dispose();
                _screenBuffer?.Dispose();

                _screenBuffer = _currentContext.Allocate(_form.CreateGraphics(), SizeFromForm);
                _screenGraphics = _screenBuffer.Graphics;

                _screenGraphics.SmoothingMode = SmoothingMode.None;
                _screenGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            }
        }

        /// <summary>
        /// Get a fresh clear bitmap buffer ready for drawing on 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Graphics GetBitmapBuffer(int width, int height)
        {
            // Create a new one if we haven't got one, or the size has changed
            if (_gameGraphics == null || _gameBuffer == null || _gameBuffer.Height != height || _gameBuffer.Width != width)
            {
                _gameGraphics?.Dispose();
                _gameBuffer?.Dispose();

                _gameBuffer = new Bitmap(width, height);
                _gameGraphics = Graphics.FromImage(_gameBuffer);

                _gameGraphics.SmoothingMode = SmoothingMode.None;
                _gameGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            }

            return _gameGraphics;
        }

        /// <summary>
        /// Transfer background on to buffer
        /// </summary>
        private void BackBufferToGameBuffer()
        {
            _gameGraphics?.DrawImageUnscaled(_backgroundBuffer, 0, 0);
        }

        /// <summary>
        /// Get a static background buffer that is used as the starting point of render
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Graphics GetBitmapBackgroundBuffer(int width, int height)
        {
            // Create a new one if we haven't got one, or the size has changed
            if (_backgroundGraphics == null || _backgroundBuffer == null || _backgroundBuffer.Height != height || _backgroundBuffer.Width != width)
            {
                _backgroundGraphics?.Dispose();
                _backgroundBuffer?.Dispose();

                _backgroundBuffer = new Bitmap(width, height);
                _backgroundGraphics = Graphics.FromImage(_backgroundBuffer);

                _backgroundGraphics.SmoothingMode = SmoothingMode.None;
                _backgroundGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            }

            return _backgroundGraphics;
        }

        public void CalculateOffsets(int width, int height)
        {
            // Scale to fit, with aspect ratio
            var scale = Math.Min((float)_formSize.Width / width,
                                 (float)_formSize.Height / height);

            // Centre in the form
            OffsetX = (_formSize.Width - width * scale) / 2;
            OffsetY = (_formSize.Height - height * scale) / 2;
        }

        /// <summary>
        /// Transfer the back buffer to the screen (resizing automatically)
        /// </summary>
        private void Render()
        {
            GetFormBuffer();

            if (_screenGraphics != null && _gameBuffer != null)
            {
                CalculateOffsets(_gameBuffer.Width, _gameBuffer.Height);

                var scale = Math.Min((float)_formSize.Width / _gameBuffer.Width,
                        (float)_formSize.Height / _gameBuffer.Height);

                _screenGraphics.DrawImage(_gameBuffer,
                    new RectangleF(OffsetX, OffsetY, _gameBuffer.Width * scale, _gameBuffer.Height * scale),
                    new RectangleF(0, 0, _gameBuffer.Width, _gameBuffer.Height),
                    GraphicsUnit.Pixel);

                // Calculate FPS if needed
                if (ShowFps)
                {
                    _fpsCounter++;
                    if (_fpsStopWatch.ElapsedMilliseconds >= 1000)
                    {
                        _fpsStopWatch.Restart();
                        _fpsLast = 1000 / _fpsCounter;
                        _fpsCounter = 0;
                    }

                    _screenGraphics.FillRectangle(_fpsBrush, 0, 0, 50, 20);
                    _screenGraphics.DrawString($"{_fpsLast} fps", _fpsFont, Brushes.White, 0, 0);
                }

            }

            _screenBuffer?.Render();
        }

        /// <summary>
        /// Render a sprite onto the specified buffer
        /// </summary>
        /// <param name="g"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="source"></param>
        private void RenderSprite(Graphics g, decimal x, decimal y, SpriteSource source)
        {
            var size = PixelGrid * source.Size;
            var pixelX = (int)((x + 0.5m - source.Size * 0.5m) * PixelGrid);
            var pixelY = (int)((y + 0.5m - source.Size * 0.5m) * PixelGrid);
            g.DrawImage(Gfx, new Rectangle(pixelX, pixelY, size, size), PixelGrid * source.XPos, PixelGrid * source.YPos,
                size, size,
                GraphicsUnit.Pixel);
        }

        /// <summary>
        /// Update the screen from the board render/display
        /// </summary>
        /// <param name="display"></param>
        public void RenderBackgroundUpdate(BoardRenderer boardRenderer)
        {
            // Update the static background with any changes
            var gBack = GetBitmapBackgroundBuffer(boardRenderer.DisplayWidth * PixelGrid,
                boardRenderer.DisplayHeight * PixelGrid);
            foreach (var sprite in boardRenderer.BackgroundToUpdate)
            {
                RenderSprite(gBack, sprite.XPos, sprite.YPos, sprite.Sprite);
            }

            // Draw the static background onto the foreground
            BackBufferToGameBuffer();

            // Draw any sprites onto the foreground
            var gSprite = GetBitmapBuffer(boardRenderer.DisplayWidth * PixelGrid,
                boardRenderer.DisplayHeight * PixelGrid);

            DrawGrid(boardRenderer, gSprite);

            foreach (var sprite in boardRenderer.SpritesToDisplay)
            {
                RenderSprite(gSprite, sprite.XPos, sprite.YPos, sprite.Sprite);
            }

            // Push out onto the screen
            Render();
        }

        private static void DrawGrid(BoardRenderer boardRenderer, Graphics gSprite)
        {
            for (int columnNumber = 0; columnNumber < boardRenderer.DisplayWidth; columnNumber++)
            {
                gSprite.DrawLine(Pens.LightGray,
                                 columnNumber * PixelGrid, 3 * PixelGrid,
                                 columnNumber * PixelGrid, boardRenderer.DisplayWidth * PixelGrid);
            }

            for (int rowNumber = 3; rowNumber <= (boardRenderer.DisplayHeight - 5); rowNumber++)
            {
                gSprite.DrawLine(Pens.LightGray,
                                 0, rowNumber * PixelGrid,
                                 (boardRenderer.DisplayWidth - 1) * PixelGrid, rowNumber * PixelGrid);
            }
        }

        private const int PixelGrid = 8;


        public void Dispose()
        {
            _currentContext?.Dispose();
            _screenBuffer?.Dispose();
            _screenGraphics?.Dispose();
            _gameBuffer?.Dispose();
            _gameGraphics?.Dispose();
            _backgroundBuffer?.Dispose();
            _backgroundGraphics?.Dispose();
        }
    }
}