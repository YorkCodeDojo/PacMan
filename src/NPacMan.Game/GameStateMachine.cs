using Automatonymous;

namespace NPacMan.Game
{
    internal class GameStateMachine :
        AutomatonymousStateMachine<GameState>
    {
        public GameStateMachine(IGameActions game, IGameSettings settings, GameNotifications gameNotifications)
        {
            InstanceState(x => x.Status);
            
            Initially(
                When(Tick)
                    .Then(context => game.MoveGhostsHome())
                    .Then(context => game.ShowGhosts(context.Instance))
                    .Then(context => game.ScatterGhosts())
                    .Then(context => gameNotifications.Publish(GameNotification.Beginning))
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(settings.InitialScatterTimeInSeconds))
                    .TransitionTo(Scatter));

            During(Scatter,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(settings.ChaseTimeInSeconds))
                    .Then(context => game.GhostToChase())
                    .TransitionTo(Alive));

            During(Alive,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(settings.InitialScatterTimeInSeconds))
                    .Then(context => game.ScatterGhosts())
                    .TransitionTo(Scatter));

            During(Scatter, Alive,
                When(Tick)
                    .ThenAsync(async context => await game.MoveGhosts(context.Data.Now))
                    .Then(context => game.MovePacMan(context, this)),
                When(CoinEaten)
                    .Then(context => context.Instance.Score += 10)
                    .Then(context => gameNotifications.Publish(GameNotification.EatCoin)),
                When(PowerPillEaten)
                    .Then(context => context.Instance.Score += 50),
                When(PacManCaughtByGhost)
                    .Then(context => context.Instance.Lives -= 1)
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(4))
                    .TransitionTo(Dying));

            WhenEnter(Dying,
                       binder => binder
                                .Then(context => gameNotifications.Publish(GameNotification.Dying)));

            During(Dying,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => game.HideGhosts(context.Instance))
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(4))
                    .IfElse(context => context.Instance.Lives > 0,
                        binder => binder.TransitionTo(Respawning),
                        binder => binder.TransitionTo(Dead)));

            WhenEnter(Respawning,
                       binder => binder
                                .Then(context => gameNotifications.Publish(GameNotification.Respawning)));

            During(Respawning,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(4))
                    .Then(context => game.MoveGhostsHome())
                    .Then(context => game.MovePacManHome())
                    .Then(context => game.ShowGhosts(context.Instance))
                    .TransitionTo(Alive));

            During(Dead, Ignore(Tick));
        }


        public State Alive { get; private set; } = null!;

        public State Scatter { get; private set; } = null!;
        public State Dying { get; private set; } = null!;
        public State Respawning { get; private set; } = null!;
        public State Dead { get; private set; } = null!;
        public Event<Tick> Tick { get; private set; } = null!;
        public Event<Tick> PacManCaughtByGhost { get; private set; } = null!;
        public Event CoinEaten { get; private set; } = null!;
        public Event PowerPillEaten { get; private set; } = null!;
    }
}