using System;
using System.Threading.Tasks;

namespace NPacMan.Game
{
    internal interface IGameActions
    {
        void MoveGhostsHome();
        void ShowGhosts(GameState gameState);
        void ScatterGhosts();
        void GhostToChase();
        Task MovePacMan(DateTime now);
        Task MoveGhosts(DateTime now);
        void HideGhosts(GameState gameState);
        void MovePacManHome();
    }
}