using System;
using System.Threading.Tasks;
using Automatonymous;

namespace NPacMan.Game
{
    internal interface IGameActions
    {
        void MoveGhostsHome();
        void ShowGhosts(GameState gameState);
        void ScatterGhosts();
        void GhostToChase();
        Task MovePacMan(BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine);
        Task MoveGhosts(DateTime now);
        void HideGhosts(GameState gameState);
        void MovePacManHome();
    }
}