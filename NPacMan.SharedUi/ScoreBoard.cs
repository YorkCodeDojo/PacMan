using System;
using System.Drawing;
using NPacMan.Game;
using NPacMan.SharedUi;

namespace NPacMan.SharedUi
{
    internal class ScoreBoard
    {
        private readonly Sprites _sprites;
        private readonly Display _display;

        public ScoreBoard(Display display, Sprites sprites)
        {
            _sprites = sprites;
            _display = display;
        }

        public void RenderStatic()
        {
            RenderString("1UP", 3, 0);
            RenderString("HIGH SCORE", 9, 0);
            RenderString("2UP", 22, 0);
        }

        public void RenderScores(int player1, int player2, int high)
        {
            RenderString(player1.ToString().PadLeft(6), 1, 1);
            RenderString(player2.ToString().PadLeft(6), 20, 1);
            RenderString(high.ToString().PadLeft(6), 11, 1);
        }

        public void RenderLivesBonus(int lives, decimal offSetY)
        {
            var lifeSprite = _sprites.PacMan(Direction.Left, 1, false);
            for (int i = 0; i < lives; i++)
            {
                _display.AddSprite(i * 2 + 2.5m, offSetY, lifeSprite);
            }
        }

        private void RenderCharacter(char c, int x, int y)
        {
            var sprite = _sprites.Character(c);
            _display.DrawOnBackground(x, y, sprite);
        }

        private void RenderDigit(int d, int x, int y)
        {
            var sprite = _sprites.Digit(d);
            _display.DrawOnBackground(x, y, sprite);
        }

        private void RenderString(string text, int x, int y)
        {
            foreach (var c in text)
            {
                if (c == ' ')
                {
                    // Don't render space
                }
                else if (Char.IsDigit(c))
                {
                    RenderDigit(c - '0', x, y);
                }
                else
                {
                    RenderCharacter(c, x, y);
                }
                x++;
            }
        }
    }
}
