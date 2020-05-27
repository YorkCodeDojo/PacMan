using NPacMan.Game;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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

    internal class Keyboard
    {
        private readonly Game.Game _game;

        public Keyboard(Game.Game game)
        {
            _game = game;
        }

        [Flags]
        private enum KeyStates
        {
            None = 0,
            Down = 1,
            Toggled = 2
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);

        private static KeyStates GetKeyState(Keys key)
        {
            KeyStates state = KeyStates.None;

            short retVal = GetKeyState((int)key);

            //If the high-order bit is 1, the key is down
            //otherwise, it is up.
            if ((retVal & 0x8000) == 0x8000)
                state |= KeyStates.Down;

            //If the low-order bit is 1, the key is toggled.
            if ((retVal & 1) == 1)
                state |= KeyStates.Toggled;

            return state;
        }

        private static bool IsKeyDown(Keys key)
        {
            return KeyStates.Down == (GetKeyState(key) & KeyStates.Down);
        }

        public async void CheckForKeyPress()
        {
            if (IsKeyDown(Keys.Left))
                await _game.ChangeDirection(Direction.Left);
            else if (IsKeyDown(Keys.Right))
                await _game.ChangeDirection(Direction.Right);
            else if (IsKeyDown(Keys.Up))
                await _game.ChangeDirection(Direction.Up);
            else if (IsKeyDown(Keys.Down))
                await _game.ChangeDirection(Direction.Down);
        }
    }
}
