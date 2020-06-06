using NPacMan.Game;

namespace NPacMan.Blazor
{
    internal static class SoundSetExtensions
    {
        internal static Game.Game AddSounds(this Game.Game game, SoundSet soundSet)
        {
            game.Subscribe(GameNotification.Beginning, soundSet.Beginning)
                .Subscribe(GameNotification.EatCoin, soundSet.Chomp)
                .Subscribe(GameNotification.Respawning, soundSet.Death)
                .Subscribe(GameNotification.EatFruit, soundSet.EatFruit)
                .Subscribe(GameNotification.EatGhost, soundSet.EatGhost)
                .Subscribe(GameNotification.ExtraPac, soundSet.ExtraPac)
                .Subscribe(GameNotification.Intermission, soundSet.Intermission);

            return game;
        }
    }
}