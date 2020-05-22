using Automatonymous;

namespace NPacMan.Game
{
    internal class GameStateMachine :
        AutomatonymousStateMachine<GameState>
    {
        public GameStateMachine(IGameSettings settings, GameNotifications gameNotifications, Game game)
        {
            InstanceState(x => x.Status);

            DuringAny(
                When(Tick)
                    .Then(context => context.Instance.RecordLastTick(context.Data.Now))
                    .Then(context => gameNotifications.Publish(GameNotification.PreTick)));

            Initially(
                When(Tick)
                    .Then(context => context.Instance.RecordLastTick(context.Data.Now))
                    .Then(context => Actions.MoveGhostsHome(context.Instance))
                    .Then(context => Actions.ShowGhosts(context.Instance))
                    .Then(context => gameNotifications.Publish(GameNotification.Beginning))
                    .TransitionTo(Scatter));

            WhenEnter(Scatter,
                       binder => binder
                       .Then(context => context.Instance.ChangeStateIn(settings.InitialScatterTimeInSeconds))
                       .Then(context => Actions.ScatterGhosts(context.Instance)));

            During(Scatter,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .TransitionTo(GhostChase));

            WhenEnter(GhostChase,
                       binder => binder
                            .Then(context => context.Instance.ChangeStateIn(settings.ChaseTimeInSeconds))
                            .Then(context => Actions.GhostToChase(context.Instance)));

            During(GhostChase,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .TransitionTo(Scatter));

            WhenEnter(Frightened,
                       binder => binder
                                .Then(context => context.Instance.ChangeStateIn(7)));

            During(Frightened,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => Actions.MakeGhostsNotEdible(context.Instance))
                    .TransitionTo(Scatter));

            During(Scatter, GhostChase, Frightened,
                When(Tick)
                    .ThenAsync(async context => await Actions.MoveGhosts(game, context.Instance, context, this))
                    .ThenAsync(async context => await Actions.MovePacMan(settings, context.Instance, context, this)),
                When(CoinCollision)
                    .Then(context => Actions.RemoveCoin(context.Instance, context.Data.Location))
                    .Then(context => context.Instance.IncreaseScore(10))
                    .Then(context => gameNotifications.Publish(GameNotification.EatCoin)),
                When(PowerPillCollision)
                    .Then(context => context.Instance.IncreaseScore(50))
                    .Then(context => gameNotifications.Publish(GameNotification.EatPowerPill))
                    .Then(context => Actions.MakeGhostsEdible(context.Instance))
                    .Then(context => Actions.RemovePowerPill(context.Instance, context.Data.Location))
                    .TransitionTo(Frightened),
                When(GhostCollision)
                    .IfElse(x => x.Data.Ghost.Edible,
                    binder => binder.Then(context =>  Actions.SendGhostHome(context.Instance, context.Data.Ghost)),
                    binder => binder.Then(context => context.Instance.DecreaseLives())
                                    .TransitionTo(Dying))); 

            WhenEnter(Dying,
                       binder => binder
                                .Then(context => Actions.HideGhosts(context.Instance))
                                .Then(context => context.Instance.ChangeStateIn(4))
                                .Then(context => gameNotifications.Publish(GameNotification.Dying)));

            During(Dying,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .IfElse(context => context.Instance.Lives > 0,
                        binder => binder.TransitionTo(Respawning),
                        binder => binder.TransitionTo(Dead)),
                Ignore(CoinCollision),
                Ignore(PowerPillCollision),
                Ignore(GhostCollision));

            WhenEnter(Respawning,
                       binder => binder
                                .Then(context => context.Instance.ChangeStateIn(4))
                                .Then(context => gameNotifications.Publish(GameNotification.Respawning)));

            During(Respawning,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => context.Instance.ChangeStateIn(4))
                    .Then(context => Actions.MoveGhostsHome(context.Instance))
                    .Then(context => Actions.MovePacManHome(context.Instance))
                    .Then(context => Actions.ShowGhosts(context.Instance))
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


    }
}