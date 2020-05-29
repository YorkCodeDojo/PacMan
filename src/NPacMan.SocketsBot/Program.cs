using NPacMan.BotSDK;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using NPacMan.BotSDK.Models;

namespace NPacMan.SocketsBot
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Please supply the hostname and port.");
                return;
            }

            ShowBanner();

            var done = false;
            while (!done)
            {
                try
                {
                    RunClient(args);
                    done = true;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Socket errored - attempting to create new socket - error was " + ex.Message);
                    Console.ResetColor();
                }
            }
        }


        private static void RunClient(string[] args)
        {
            var hostname = args[0];
            var port = int.Parse(args[1]);

            var ipHostInfo = Dns.GetHostEntry(hostname);
            var ipAddress = ipHostInfo.AddressList[0];
            var localEndPoint = new IPEndPoint(ipAddress, port);

            var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            byte[] bytes = new Byte[1024];

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    var handler = listener.Accept();

                    var done = false;
                    GreedyBot? bot = null;
                    while (!done)
                    {
                        var command = string.Empty;
                        while (true)
                        {
                            int bytesRec = handler.Receive(bytes);
                            command += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            if (command.IndexOf("<EOF>") > -1)
                            {
                                command = command[..(command.Length - 5)];
                                break;
                            }
                        }

                        if (command.StartsWith("exit:"))
                        {
                            Console.WriteLine("Exit");
                            done = true;
                        }
                        else if (command.StartsWith("initialise:"))
                        {
                            Console.WriteLine("Initialise");
                            var boardState = command[("initialise:".Length)..];
                            var board = JsonSerializer.Deserialize<BotBoard>(boardState);
                            bot = new GreedyBot(board);

                            var msg = Encoding.ASCII.GetBytes("ok");
                            handler.Send(msg);
                        }
                        else if (command.StartsWith("play:"))
                        {
                            Console.WriteLine("Play");
                            var gameState = command[("play:".Length)..];
                            var game = JsonSerializer.Deserialize<BotGame>(gameState);
                            if (bot is null) bot = new GreedyBot(game.Board);
                            var nextDirection = bot.SuggestNextDirection(game).ToString().ToLower();
                            Console.WriteLine($"    Responding with {nextDirection}");

                            var msg = Encoding.ASCII.GetBytes(nextDirection);
                            handler.Send(msg);
                        }
                    }
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    handler.Dispose();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                listener.Shutdown(SocketShutdown.Both);
                listener.Dispose();
                throw;
            }


        }

        private static void ShowBanner()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(@"
  _____           __  __               ____        _   
 |  __ \         |  \/  |             |  _ \      | |         ▒▒▒▒▒    ▒▒▒▒▒             ▄████▄     
 | |__) |_ _  ___| \  / | __ _ _ __   | |_) | ___ | |_       ▒ ▄▒ ▄▒  ▒ ▄▒ ▄▒           ███▄█▀      
 |  ___/ _` |/ __| |\/| |/ _` | '_ \  |  _ < / _ \| __|      ▒▒▒▒▒▒▒  ▒▒▒▒▒▒▒          ▐████  █  █  
 | |  | (_| | (__| |  | | (_| | | | | | |_) | (_) | |_       ▒▒▒▒▒▒▒  ▒▒▒▒▒▒▒           █████▄      
 |_|   \__,_|\___|_|  |_|\__,_|_| |_| |____/ \___/ \__|      ▒ ▒ ▒ ▒  ▒ ▒ ▒ ▒            ▀████▀     


Using Sockets:
");
            Console.ResetColor();
        }
    }
}
