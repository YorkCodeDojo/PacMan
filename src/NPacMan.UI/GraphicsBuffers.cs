using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NPacMan.SharedUi;

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

        public GraphicsBuffers(Form forms)
        {
            _form = forms;
            _currentContext = BufferedGraphicsManager.Current;
            _formSize = _form.DisplayRectangle;

            _fpsStopWatch=new Stopwatch();
            _fpsStopWatch.Start();

            Gfx = new Bitmap("gfx.png");

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

        /// <summary>
        /// Transfer the back buffer to the screen (resizing automatically)
        /// </summary>
        private void Render()
        {
            GetFormBuffer();

            if (_screenGraphics != null && _gameBuffer != null)
            {
                // Scale to fit, with aspect ratio
                var scale = Math.Min((float) _formSize.Width / _gameBuffer.Width,
                    (float) _formSize.Height / _gameBuffer.Height);

                // Centre in the form
                var x = (_formSize.Width - _gameBuffer.Width * scale) / 2;
                var y = (_formSize.Height - _gameBuffer.Height * scale) / 2;

                _screenGraphics.DrawImage(_gameBuffer,
                    new RectangleF(x, y, _gameBuffer.Width * scale, _gameBuffer.Height * scale),
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
        public void RenderBackgroundUpdate(Display display)
        {
            // Update the static background with any changes
            var gBack = GetBitmapBackgroundBuffer(display.Width * PixelGrid, display.Height * PixelGrid);
            foreach (var sprite in display.GetBackgroundToDraw())
            {
                RenderSprite(gBack, sprite.XPos, sprite.YPos, sprite.Sprite);
            }

            // Draw the static background onto the foreground
            BackBufferToGameBuffer();

            // Draw any sprites onto the foreground
            var gSprite = GetBitmapBuffer(display.Width * PixelGrid, display.Height * PixelGrid);
            foreach (var sprite in display.SpritesToDisplay)
            {
                RenderSprite(gSprite, sprite.XPos, sprite.YPos, sprite.Sprite);
            }

            // Push out onto the screen
            Render();
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