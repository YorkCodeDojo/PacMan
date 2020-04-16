using System;
using System.Linq;

namespace NPacMan.GenerateBoard
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             *   X = wall
             *   . = coin
             *     = empty
             */

            var board = @"
XXXXXXXXXXXXXXXXXXXXXXXXXXXX
X............XX............X
X.XXXX.XXXXX.XX.XXXXX.XXXX.X
X.X  X.X   X.XX.X   X.X  X.X
X.XXXX.XXXXX.XX.XXXXX.XXXX.X
X..........................X
X.XXXX.XX.XXXXXXXX.XX.XXXX.X
X.XXXX.XX.XXXXXXXX.XX.XXXX.X
X......XX....XX....XX......X
XXXXXX.XXXXX XX XXXXX.XXXXXX
     X.XXXXX XX XXXXX.X     
     X.XX          XX.X     
     X.XX          XX.X     
XXXXXX.XX  XXXXXX  XX.XXXXXX
      .XX  X    X  XX.      
      .XX  X    X  XX.      
XXXXXX.XX  XXXXXX  XX.XXXXXX
     X.XX          XX.X     
     X.XX          XX.X     
XXXXXX.XX XXXXXXXX XX.XXXXXX
X............XX............X
X.XXXX.XXXXX.XX.XXXXX.XXXX.X
X.XXXX.XXXXX.XX.XXXXX.XXXX.X
X...XX................XX...X
XXX.XX.XX.XXXXXXXX.XX.XX.XXX
XXX.XX.XX.XXXXXXXX.XX.XX.XXX
X......XX....XX....XX......X
X.XXXXXXXXXX.XX.XXXXXXXXXX.X
X.XXXXXXXXXX.XX.XXXXXXXXXX.X
X..........................X
XXXXXXXXXXXXXXXXXXXXXXXXXXXX
";

            var rows = board.Split(System.Environment.NewLine);
            rows = rows.Skip(1).ToArray();

            for (int rowNumber = 0; rowNumber < rows.Length; rowNumber++)
            {
                var row = rows[rowNumber];
                for (int columnNumber = 0; columnNumber < row.Length; columnNumber++)
                {
                    var cell = row[columnNumber];
                    if (cell == 'X')
                    {
                        Console.Write($"({columnNumber},{rowNumber}),");
                    }
                }
                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}
