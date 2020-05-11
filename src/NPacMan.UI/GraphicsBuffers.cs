using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NPacMan.UI
{
    public class GraphicsBuffers : IDisposable
    {
        private BufferedGraphicsContext _currentContext;

        private BufferedGraphics? _screenBuffer;
        private Graphics? _screenGraphics;

        private Bitmap? _gameBuffer;
        private Graphics? _gameGraphics;

        private readonly Form _form;

        private Rectangle _formSize;

        private Rectangle SizeFromForm => _form.ClientRectangle;

        public bool ShowFps = false;

        private readonly Stopwatch _fpsStopWatch;
        private int _fpsCounter = 0;
        private int _fpsLast = 0;

        private readonly Font _fpsFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
        private readonly Brush _fpsBrush = new SolidBrush(Color.DarkRed); 

        public GraphicsBuffers(Form forms)
        {
            _form = forms;
            _currentContext = BufferedGraphicsManager.Current;
            _formSize = _form.DisplayRectangle;

            _fpsStopWatch=new Stopwatch();
            _fpsStopWatch.Start();
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
        public Graphics GetBitmapBuffer(int width, int height)
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

            // Clear it and reset origin
            _gameGraphics.TranslateTransform(0, 0);
            _gameGraphics.Clear(Color.Black);

            return _gameGraphics;
        }

        /// <summary>
        /// Transfer the back buffer to the screen (resizing automatically)
        /// </summary>
        public void Render()
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

        public void Dispose()
        {
            _currentContext?.Dispose();
            _screenBuffer?.Dispose();
            _screenGraphics?.Dispose();
            _gameBuffer?.Dispose();
            _gameGraphics?.Dispose();
        }
    }
}