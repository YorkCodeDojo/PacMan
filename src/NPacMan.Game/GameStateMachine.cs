using Automatonymous;
using System.Linq;

namespace NPacMan.Game
{
    internal class GameStateMachine :
        AutomatonymousStateMachine<GameState>
    {
        public GameStateMachine(IGameActions game, IGameSettings settings, GameNotifications gameNotifications)
        {
            InstanceState(x => x.Status);

            DuringAny(
                When(Tick)
                    .Then(context => context.Instance.LastTick = context.Data.Now));

            Initially(
                When(Tick)
                    .Then(context => context.Instance.LastTick = context.Data.Now)
                    .Then(context => game.MoveGhostsHome())
                    .Then(context => ShowGhosts(context.Instance))
                    .Then(context => gameNotifications.Publish(GameNotification.Beginning))
                    .TransitionTo(Scatter));

            WhenEnter(Scatter,
                       binder => binder
                       .Then(context => context.Instance.TimeToChangeState = context.Instance.LastTick.AddSeconds(settings.InitialScatterTimeInSeconds))
                       .Then(context => game.ScatterGhosts()));

            During(Scatter,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .TransitionTo(GhostChase));

            WhenEnter(GhostChase,
                       binder => binder
                            .Then(context => context.Instance.TimeToChangeState = context.Instance.LastTick.AddSeconds(settings.ChaseTimeInSeconds))
                            .Then(context => game.GhostToChase()));

            During(GhostChase,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .TransitionTo(Scatter));

            WhenEnter(Frightened,
                       binder => binder
                                .Then(context => context.Instance.TimeToChangeState = context.Instance.LastTick.AddSeconds(7)));

            During(Frightened,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(x => game.MakeGhostsNotEdible())
                    .TransitionTo(Scatter));

            During(Scatter, GhostChase, Frightened,
                When(Tick)
                    .ThenAsync(async context => await game.MoveGhosts(context, this))
                    .Then(context => game.MovePacMan(context, this)),
                When(CoinCollision)
                    .Then(context => context.Instance.Score += 10)
                    .Then(context => gameNotifications.Publish(GameNotification.EatCoin))
                    .Then(context => RemoveCoin(context.Instance, context.Data.Location)),
                When(PowerPillCollision)
                    .Then(context => context.Instance.Score += 50)
                    .Then(context => gameNotifications.Publish(GameNotification.EatPowerPill))
                    .Then(context => game.MakeGhostsEdible())
                    .Then(context => RemovePowerPill(context.Instance, context.Data.Location))
                    .TransitionTo(Frightened),
                When(GhostCollision)
                    .IfElse(x => x.Data.Ghost.Edible,
                    binder => binder.Then(x => game.SendGhostHome(x.Data.Ghost)),
                    binder => binder.Then(context => context.Instance.Lives -= 1)
                                    .TransitionTo(Dying))); ;

            WhenEnter(Dying,
                       binder => binder
                                .Then(context => HideGhosts(context.Instance))
                                .Then(context => context.Instance.TimeToChangeState = context.Instance.LastTick.AddSeconds(4))
                                .Then(context => gameNotifications.Publish(GameNotification.Dying)));

            During(Dying,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .IfElse(context => context.Instance.Lives > 0,
                        binder => binder.TransitionTo(Respawning),
                        binder => binder.TransitionTo(Dead)));

            WhenEnter(Respawning,
                       binder => binder
                                .Then(context => context.Instance.TimeToChangeState = context.Instance.LastTick.AddSeconds(4))
                                .Then(context => gameNotifications.Publish(GameNotification.Respawning)));

            During(Respawning,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(4))
                    .Then(context => game.MoveGhostsHome())
                    .Then(context => game.MovePacManHome())
                    .Then(context => ShowGhosts(context.Instance))
                    .TransitionTo(GhostChase));

            During(Dead, Ignore(Tick));
        }

        public State GhostChase { get; private set; } = null!;
        public State Scatter { get; private set; } = null!;
        public State Frightened { get; private set; } = null!;
        public State Dying { get; private set; } = null!;
        public State Respawning { get; private set; } = null!;
        public State Dead { get; private set; } = null!;
        public Event<Tick> Tick { get; private set; } = null!;
        public Event<GhostCollision> GhostCollision { get; private set; } = null!;
        public Event<CoinCollision> CoinCollision { get; private set; } = null!;
        public Event<PowerPillCollision> PowerPillCollision { get; private set; } = null!;

        void ShowGhosts(GameState gameState)
        {
            gameState.GhostsVisible = true;
        }

        void HideGhosts(GameState gameState)
        {
            gameState.GhostsVisible = false;
        }

        void RemoveCoin(GameState gameState, CellLocation location)
        {
            // Note - this is not the same as gameState.RemainingCoins = gameState.RemainingCoins.Remove(location)
            // We have to allow for the UI to be iterating over the list whilst we are removing elements from it.
            gameState.RemainingCoins = gameState.RemainingCoins.Where(c => c != location).ToList();
        }

        void RemovePowerPill(GameState gameState, CellLocation location)
        {
            // Note - this is not the same as gameState.RemainingPowerPills = gameState.RemainingPowerPills.Remove(location)
            // We have to allow for the UI to be iterating over the list whilst we are removing elements from it.
            gameState.RemainingPowerPills = gameState.RemainingPowerPills.Where(p => p != location).ToList();
        }
    }
}