using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace NPacMan.SharedUi
{
    public class Display
    {
        private SpriteSource[,] _screen;
        private SpriteSource[,] _buffer;

        public int Height { get; private set; }
        public int Width { get; private set; }

        public Display()
        {
            _screen = new SpriteSource[1,1];
            _buffer=new SpriteSource[1,1];
            _spritesToDisplay= new List<SpriteDisplay>();

            Height = 1;
            Width = 1;
        }

        public void Size(int width, int height)
        {
            _spritesToDisplay = new List<SpriteDisplay>();
            if (width != Width || height != Height)
            {
                Height = height;
                Width = width;

                _screen = new SpriteSource[Width, Height];
                _buffer = new SpriteSource[Width, Height];
            }
        }

        public IEnumerable<SpriteDisplay> GetBackgroundToDraw()
        {
            var toDraw = new List<SpriteDisplay>();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var s = _screen[x, y];
                    var b = _buffer[x, y];
                    if (s != b)
                    {
                        _buffer[x, y] = s;
                        toDraw.Add(new SpriteDisplay(x, y, s));
                    }
                }
            }

            return toDraw;
        }

        public void DrawOnBackground(int x, int y, SpriteSource sprite)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                _screen[x, y] = sprite;
        }

        public void AddSprite(decimal x, decimal y, SpriteSource sprite)
        {
            _spritesToDisplay.Add(new SpriteDisplay(x, y, sprite));
        }

        private List<SpriteDisplay> _spritesToDisplay;

        public IEnumerable<SpriteDisplay> SpritesToDisplay => _spritesToDisplay.AsReadOnly();
    }
}
