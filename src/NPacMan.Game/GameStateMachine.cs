using Automatonymous;

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
                    .Then(context => game.ShowGhosts(context.Instance))
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

            During(Frightened,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(x => game.MakeGhostsNotEdible())
                    .TransitionTo(Scatter));

            During(Scatter, GhostChase, Frightened,
                When(Tick)
                    .ThenAsync(async context => await game.MoveGhosts(context.Data.Now))
                    .Then(context => game.MovePacMan(context, this)),
                When(CoinEaten)
                    .Then(context => context.Instance.Score += 10)
                    .Then(context => gameNotifications.Publish(GameNotification.EatCoin)),
                When(PowerPillEaten)
                    .Then(context => context.Instance.Score += 50)
                    .Then(context => gameNotifications.Publish(GameNotification.EatPowerPill))
                    .Then(context => game.MakeGhostsEdible())
                    .Then(context => context.Instance.TimeToChangeState = context.Instance.LastTick.AddSeconds(7))
                    .TransitionTo(Frightened),
                When(PacManCaughtByGhost)
                    .Then(context => context.Instance.Lives -= 1)
                    .TransitionTo(Dying));

            WhenEnter(Dying,
                       binder => binder
                                .Then(context => context.Instance.TimeToChangeState = context.Instance.LastTick.AddSeconds(4))
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
        public Event<Tick> PacManCaughtByGhost { get; private set; } = null!;
        public Event CoinEaten { get; private set; } = null!;
        public Event PowerPillEaten { get; private set; } = null!;
    }
}