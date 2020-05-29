using NPacMan.BotSDK;
using System;
using System.Text.Json;
using NPacMan.BotSDK.Models;

namespace NPacMan.StdInOutBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var done = false;
            GreedyBot? bot = null;

            while (!done)
            {
                var command = Console.ReadLine();

                if (command.StartsWith("exit:"))
                {
                    Console.WriteLine("Exit");
                    done = true;
                }
                else if (command.StartsWith("initialise:"))
                {
                    var boardState = command[("initialise:".Length)..];
                    var board = JsonSerializer.Deserialize<BotBoard>(boardState);
                    bot = new GreedyBot(board);
                    Console.WriteLine("ok");
                }
                else if (command.StartsWith("play:"))
                {
                    var gameState = command[("play:".Length)..];
                    var game = JsonSerializer.Deserialize<BotGame>(gameState);
                    if (bot is null) bot = new GreedyBot(game.Board);
                    var nextDirection = bot.SuggestNextDirection(game).ToString().ToLower();
                    Console.WriteLine(nextDirection);
                }
            }
        }
    }
}
