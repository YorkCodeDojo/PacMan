using System;
using NPacMan.Game;
using NPacMan.SharedUi.Map;

namespace NPacMan.SharedUi
{
    internal class Sprites
    {
        public Sprites()
        {
            Bonus = new SpriteSource[8];
            for (int i = 0; i < 8; i++)
                Bonus[i] = new SpriteSource(i * 2, 10, 2);
        }

        public SpriteSource[] Bonus;

        public SpriteSource Coin() => new SpriteSource(16, 0, 1);

        public SpriteSource PowerPill() => new SpriteSource(20, 0, 1);

        /// <summary>
        /// Get source of ghost sprite
        /// </summary>
        /// <param name="ghostColour"></param>
        /// <param name="direction"></param>
        /// <param name="animated"></param>
        /// <returns></returns>
        public SpriteSource Ghost(GhostColour ghostColour, Direction direction, bool animated)
        {
            int xpos;
            int ypos;
            switch (ghostColour)
            {
                case GhostColour.Red:
                    xpos = 0;
                    ypos = 14;
                    break;
                case GhostColour.Cyan:
                    xpos = 16;
                    ypos = 18;
                    break;
                case GhostColour.Pink:
                    xpos = 0;
                    ypos = 20;
                    break;
                case GhostColour.Orange:
                    xpos = 16;
                    ypos = 20;
                    break;
                case GhostColour.Eyes:
                    xpos = 0;
                    ypos = 22;
                    break;
                case GhostColour.BlueFlash:
                    xpos = 24;
                    ypos = 12;
                    break;
                case GhostColour.WhiteFlash:
                    xpos = 28;
                    ypos = 12;
                    break;
                default:
                    throw new Exception("GhostColour?");
            }

            if(ghostColour != GhostColour.BlueFlash && ghostColour != GhostColour.WhiteFlash)
            {
                switch (direction)
                {
                    case Direction.Up:
                        xpos += 12;
                        break;
                    case Direction.Down:
                        xpos += 4;
                        break;
                    case Direction.Left:
                        xpos += 8;
                        break;
                    case Direction.Right:
                        break;
                    default:
                        throw new Exception("Direction?");
                }
            }

            if (animated)
                xpos += 2;

            return new SpriteSource(xpos, ypos, 2);

        }

        /// <summary>
        /// Points for eating ghosts 200, 400, 800, 1600
        /// </summary>
        /// <param name="multiplier">0-3</param>
        /// <returns></returns>
        public SpriteSource GhostPoints(int multiplier)
        {
            return new SpriteSource(16 + multiplier * 2, 14, 2);
        }

        /// <summary>
        /// Text number (hexadecimal)
        /// </summary>
        /// <param name="number">0-9</param>
        /// <returns></returns>
        public SpriteSource Digit(int number)
        {
            return new SpriteSource(16 + number, 1, 1);
        }

        /// <summary>
        /// Text character upper case letters
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public SpriteSource Character(char character)
        {
            if (character < 'A' || character > 'Z') return new SpriteSource(0, 2, 1);
            if (character == '!') return new SpriteSource(27, 2, 1);
            return new SpriteSource(character - 64, 2, 1);
        }

        /// <summary>
        /// Get source of Pacmac sprite
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="animation">0-3 moving, 0-11 dying</param>
        /// <param name="dying"></param>
        /// <returns></returns>
        public SpriteSource PacMan(Direction direction, int animation, bool dying)
        {
            if (dying)
            {
                return new SpriteSource(8 + animation - 4 * 2, 16, 2);
            }

            if (animation == 2)
            {
                return new SpriteSource(0,16,2);
            }

            int xpos;
            int ypos = 18;
            switch (direction)
            {
                case Direction.Right:
                    xpos = 0;
                    break;
                case Direction.Down:
                    xpos = 2;
                    break;
                case Direction.Left:
                    xpos = 6;
                    break;
                case Direction.Up:
                    xpos = 4;
                    break;
                default:
                    throw new Exception("Direction?");
            }

            if (animation == 1 || animation == 3)
                xpos += 8;

            return new SpriteSource(xpos, ypos, 2);
        }

        public SpriteSource Map(BoardPiece piece)
        {
            switch (piece)
            {
                case BoardPiece.Blank:
                    return new SpriteSource(0, 6, 1);
                case BoardPiece.Pill:
                    return new SpriteSource(16, 0, 1);
                case BoardPiece.PowerAnim1:
                    return new SpriteSource(18, 0, 1);
                case BoardPiece.PowerAnim2:
                    return new SpriteSource(20, 0, 1);
                case BoardPiece.DoubleTopRight:
                    return new SpriteSource(16, 6, 1);
                case BoardPiece.DoubleTopLeft:
                    return new SpriteSource(17, 6, 1);
                case BoardPiece.DoubleRight:
                    return new SpriteSource(18, 6, 1);
                case BoardPiece.DoubleLeft:
                    return new SpriteSource(19, 6, 1);
                case BoardPiece.DoubleBottomRight:
                    return new SpriteSource(20, 6, 1);
                case BoardPiece.DoubleBottomLeft:
                    return new SpriteSource(21, 6, 1);
                case BoardPiece.JoinRightHandTop:
                    return new SpriteSource(22, 6, 1);
                case BoardPiece.JoinLeftHandTop:
                    return new SpriteSource(23, 6, 1);
                case BoardPiece.JoinRightHandBottom:
                    return new SpriteSource(24, 6, 1);
                case BoardPiece.JoinLeftHandBottom:
                    return new SpriteSource(25, 6, 1);
                case BoardPiece.DoubleTop:
                    return new SpriteSource(26, 6, 1);
                case BoardPiece.DoubleBottom:
                    return new SpriteSource(28, 6, 1);
                case BoardPiece.Top:
                    return new SpriteSource(30, 6, 1);
                case BoardPiece.Bottom:
                    return new SpriteSource(4, 7, 1);
                case BoardPiece.TopRight:
                    return new SpriteSource(6, 7, 1);
                case BoardPiece.TopLeft:
                    return new SpriteSource(7, 7, 1);
                case BoardPiece.Right:
                    return new SpriteSource(8, 7, 1);
                case BoardPiece.Left:
                    return new SpriteSource(9, 7, 1);
                case BoardPiece.BottomRight:
                    return new SpriteSource(10, 7, 1);
                case BoardPiece.BottomLeft:
                    return new SpriteSource(11, 7, 1);
                case BoardPiece.GhostTopRight:
                    return new SpriteSource(12, 7, 1);
                case BoardPiece.GhostTopLeft:
                    return new SpriteSource(13, 7, 1);
                case BoardPiece.GhostBottomRight:
                    return new SpriteSource(14, 7, 1);
                case BoardPiece.GhostBottomLeft:
                    return new SpriteSource(15, 7, 1);
                case BoardPiece.GhostEndRight:
                    return new SpriteSource(16, 7, 1);
                case BoardPiece.GhostEndLeft:
                    return new SpriteSource(17, 7, 1);
                case BoardPiece.JoinTopRight:
                    return new SpriteSource(26, 7, 1);
                case BoardPiece.JoinTopLeft:
                    return new SpriteSource(27, 7, 1);
                case BoardPiece.GhostDoor:
                    return new SpriteSource(15, 6, 1);
                case BoardPiece.InnerTopRight:
                    return new SpriteSource(19, 7, 1);
                case BoardPiece.InnerTopLeft:
                    return new SpriteSource(18, 7, 1);
                case BoardPiece.InnerBottomRight:
                    return new SpriteSource(21, 7, 1);
                case BoardPiece.InnerBottomLeft:
                    return new SpriteSource(20, 7, 1);
                case BoardPiece.InsideWalls:
                    return new SpriteSource(1, 6, 1);
                case BoardPiece.Undefined:
                    return new SpriteSource(13, 6, 1);
                case BoardPiece.JoinBottomRight:
                    return new SpriteSource(30, 7, 1);
                case BoardPiece.JoinBottomLeft:
                    return new SpriteSource(31, 7, 1);
                default:
                    throw new Exception("Bad board piece");
            }
        }
    }
}