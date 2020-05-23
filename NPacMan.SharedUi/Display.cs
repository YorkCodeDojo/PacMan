using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace NPacMan.SharedUi
{
    public class Display
    {
        private SpriteSource[,] _screen;
        private SpriteSource[,] _buffer;

        private int _height;
        private int _width;

        public Display()
        {
            _screen = new SpriteSource[1,1];
            _buffer=new SpriteSource[1,1];

            _height = 1;
            _width = 1;
        }

        public void Size(int width, int height)
        {
            if (width != _width || height != _height)
            {
                _height = height;
                _width = width;

                _screen = new SpriteSource[_width, _height];
                _buffer = new SpriteSource[_width, _height];
            }
        }

        private IEnumerable<SpriteDisplay> GetBackgroundToDraw()
        {
            var toDraw = new List<SpriteDisplay>();
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
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
            if (x >= 0 && x < _width && y >= 0 && y < _height)
                _screen[x, y] = sprite;
        }
    }
}
