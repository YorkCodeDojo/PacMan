using NPacMan.Game;

namespace NPacMan.UI
{
    internal static class KeyboardExtension
    {
        public static Game.Game AddKeyboard(this Game.Game game)
        {
            var keyboard = new Keyboard(game);
            game.Subscribe(GameNotification.PreTick, keyboard.CheckForKeyPress);

            return game;
        }
    }
}