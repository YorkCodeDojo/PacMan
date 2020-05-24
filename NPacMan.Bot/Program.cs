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
            if (ss.ReadString() == "I am the one true server!")
            {
                // The client security token is sent with the first write.
                // Send the name of the file whose contents are returned
                // by the server.

                GreedyBot? bot = null;
                var done = false;
                while (!done)
                {
                    var gameState = ss.ReadString();
                    if (gameState.Equals("exit"))
                    {
                        done = true;
                    }
                    else
                    {
                        try
                        {
                            var game = JsonSerializer.Deserialize<BotGame>(gameState);

                            if (bot is null)
                            {
                                Console.WriteLine("First request - building bot");
                                bot = new GreedyBot(game);
                            }

                            Console.WriteLine($"PacMan is currently at {game.PacMan}");
                            var nextDirection = bot.SuggestNextDirection(game).ToString().ToLower();

                            Console.WriteLine($"Suggesting {nextDirection}");

                            ss.WriteString(nextDirection);
                        }
                        catch (System.Text.Json.JsonException ex)
                        {
                            Console.WriteLine(">> Sent Rubbish? " + ex.Message);
                        }


                    }
                }
            }
            else
            {
                Console.WriteLine("Server could not be verified.");
            }
            pipeClient.Close();
            // Give the client process some time to display results before exiting.
            Thread.Sleep(4000);

        }
    }
}
