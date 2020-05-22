using NPacMan.Game;

namespace NPacMan.UI
{
    internal interface IBot
    {
        Direction SuggestNextDirection(Game.Game game);
    }
}