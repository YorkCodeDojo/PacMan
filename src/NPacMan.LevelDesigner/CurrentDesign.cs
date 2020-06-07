using NPacMan.Game;
using NPacMan.UI;

namespace NPacMan.LevelDesigner
{
    public class CurrentDesign
    {
        public int Width => 30;
        public int Height => 30;

        private readonly char[,] _board;

        public CurrentDesign()
        {
            _board = new char[Width, Height];
            _board[0, 0] = 'X';
            _board[1, 0] = 'X';
            _board[2, 0] = 'X';
            _board[3, 0] = 'X';
            _board[0, 1] = 'X';
            _board[0, 2] = 'X';
            _board[0, 3] = 'X';
        }

        public IGameSettings GameSettingsForDesign()
        {
            var currentDesign = new GameSettingsForCurrentDesign
            {
                InitialLives = 0,
                Height = 30,
                Width = 30
            };

            CellLocation fruit = new CellLocation(10,10);
            PacMan pacMan = new PacMan(new CellLocation(11, 11), Direction.Up);

            for (int rowNumber = 0; rowNumber < Height; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber < Width; columnNumber++)
                {
                    var location = new CellLocation(columnNumber, rowNumber);
                    switch (_board[columnNumber, rowNumber])
                    {
                        case '▲':
                            pacMan = new PacMan(location, Direction.Up);
                            break;
                        case '▼':
                            pacMan = new PacMan(location, Direction.Down);
                            break;
                        case '►':
                            pacMan = new PacMan(location, Direction.Right);
                            break;
                        case '◄':
                            pacMan = new PacMan(location, Direction.Left);
                            break;
                        case 'X':
                            currentDesign.Walls.Add(location);
                            break;
                        case '-':
                            currentDesign.Doors.Add(location);
                            break;
                        case 'T':
                            //currentDesign.Portals.Add(location);
                            break;
                        case '.':
                            currentDesign.Coins.Add(location);
                            break;
                        case '*':
                            currentDesign.PowerPills.Add(location);
                            break;
                        case 'H':
                            currentDesign.GhostHouse.Add(location);
                            break;
                        case 'F':
                            fruit = location;
                            break;
                        default:
                            break;
                    }
                }
            }

            currentDesign.Fruit = fruit;
            currentDesign.PacMan = pacMan;

            return currentDesign;
        }

        internal void AddPacManLeft(int x, int y)
        {
            _board[x, y] = '◄';
        }
        internal void AddPacManRight(int x, int y)
        {
            _board[x, y] = '►';
        }
        internal void AddPacManUp(int x, int y)
        {
            _board[x, y] = '▲';
        }
        internal void AddPacManDown(int x, int y)
        {
            _board[x, y] = '▼';
        }
        internal void AddDoor(int x, int y)
        {
            _board[x, y] = '-';
        }
        internal void AddPortal(int x, int y)
        {
            _board[x, y] = 'T';
        }
        internal void AddCoin(int x, int y)
        {
            _board[x, y] = '.';
        }
        internal void AddWall(int x, int y)
        {
            _board[x, y] = 'X';
        }
        internal void AddPowerPill(int x, int y)
        {
            _board[x, y] = '*';
        }
        internal void AddGhostHouse(int x, int y)
        {
            _board[x, y] = 'H';
        }
        internal void AddFruit(int x, int y)
        {
            _board[x, y] = 'F';
        }
    }
}
