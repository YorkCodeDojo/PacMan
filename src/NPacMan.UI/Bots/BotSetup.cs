using NPacMan.Game;
using NPacMan.UI.Bots.SocketTransport;

namespace NPacMan.UI.Bots
{
    internal static class BotSetup
    {
        public static Game.Game AddBots(this Game.Game game, string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].Equals("--pipename"))
                {
                    var pipename = args[1];
                    var transport = new NamedPipesTransport(pipename);

                    var botConnector = new BotConnector(transport, game);
                    botConnector.Initialise();

                    game.Subscribe(GameNotification.PreTick, botConnector.RequestNextMoveFromBot);
                }

                if (args[0].Equals("--hostname"))
                {
                    var hostNameAndPortal = args[1];
                    var parts = hostNameAndPortal.Split(':');
                    var transport = new SocketsTransport(parts[0], int.Parse(parts[1]));

                    var botConnector = new BotConnector(transport, game);
                    botConnector.Initialise();

                    game.Subscribe(GameNotification.PreTick, botConnector.RequestNextMoveFromBot);
                }
            }

            return game;
        }
    }
}
