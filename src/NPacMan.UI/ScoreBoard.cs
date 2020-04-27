using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NPacMan.UI;

namespace NPacMan.Game
{
    public class ScoreBoard
    {
        private Sprites _sprites;

        public ScoreBoard(Sprites sprites)
        {
            _sprites = sprites;
        }

        public void RenderStatic(Graphics g)
        {
            RenderString(g, "1UP", 3, 0);
            RenderString(g, "HIGH SCORE", 9, 0);
            RenderString(g, "2UP", 22, 0);
        }

        public void RenderScores(Graphics g, int player1, int player2, int high)
        {
            RenderString(g, player1.ToString().PadLeft(6), 1, 1);
            RenderString(g, player2.ToString().PadLeft(6), 20, 1);
            RenderString(g, high.ToString().PadLeft(6), 11, 1);
        }

        public void RenderLivesBonus(Graphics g, int lives, int boardHeight)
        {
            var lifeSprite = _sprites.PacMan(Direction.Left, 1, false);
            // The large sprite in the score board span two grid squares
            // rather than being central to one. Adding 0.5 grid square to compensate
            var ypos = boardHeight * Sprites.PixelGrid + Sprites.PixelGrid / 2;
            for (int i = 0; i < lives; i++)
            {
                _sprites.RenderSprite(g, (i * Sprites.PixelGrid * 2) + 2 * Sprites.PixelGrid + Sprites.PixelGrid / 2, ypos,  lifeSprite);
            }
        }

        private void RenderCharacter(Graphics g, char c, int x, int y)
        {
            var sprite = _sprites.Character(c);
            _sprites.RenderSprite(g, x * Sprites.PixelGrid, y * Sprites.PixelGrid, sprite);
        }

        private void RenderDigit(Graphics g, int d, int x, int y)
        {
            var sprite = _sprites.Digit(d);
            _sprites.RenderSprite(g, x * Sprites.PixelGrid, y * Sprites.PixelGrid, sprite);
        }

        private void RenderString(Graphics g, string text, int x, int y)
        {
            foreach (var c in text)
            {
                if (c == ' ')
                {
                    // Don't render space
                }
                else if (Char.IsDigit(c))
                {
                    RenderDigit(g, c - '0', x, y);
                }
                else
                {
                    RenderCharacter(g, c, x, y);
                }
                x++;
            }
        }
    }
}
