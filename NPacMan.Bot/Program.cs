using NPacMan.BotSDK;
using System;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text.Json;
using System.Threading;

namespace NPacMan.Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please supply the ClientHandle.");
                return;
            }

            using var pipeClient = new NamedPipeClientStream(".", args[0], PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);
            Console.WriteLine("Connecting to server...\n");
            pipeClient.Connect();

            var ss = new StreamString(pipeClient);
            GreedyBot? bot = null;
            var done = false;
            while (!done)
            {
                var command = ss.ReadString();

                if (command.StartsWith("exit:"))
                {
                    Console.WriteLine("Exit");
                    done = true;
                }
                else if (command.StartsWith("initialise:"))
                {
                    Console.WriteLine("Initialise");
                    var boardState = command[("initialise:".Length)..];
                    Console.WriteLine(boardState.Substring(0,10));
                    var board = JsonSerializer.Deserialize<BotBoard>(boardState);
                    bot = new GreedyBot(board);
                }
                else if (command.StartsWith("play:"))
                {
                    Console.WriteLine("Play");
                    var gameState = command[("play:".Length)..];
                    var game = JsonSerializer.Deserialize<BotGame>(gameState);
                    if (bot is null) bot = new GreedyBot(game.Board);
                    var nextDirection = bot.SuggestNextDirection(game).ToString().ToLower();
                    Console.WriteLine($"    Responding with {nextDirection}");
                    ss.WriteString(nextDirection);
                }
            }

            pipeClient.Close();
            // Give the client process some time to display results before exiting.
            Thread.Sleep(4000);

        }
    }
}
