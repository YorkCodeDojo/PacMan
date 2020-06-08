using Automatonymous;
using System;

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
                    .Then(context => Actions.Tick(context.Instance, context.Data.Now, gameNotifications)));

            Initially(
                When(Tick)
                    .Then(context => Actions.SetupGame(context.Instance, context.Data.Now, gameNotifications))
                    .TransitionTo(Scatter));

            WhenEnter(Scatter,
                       binder => binder
                       .Then(context => context.Instance.ChangeStateIn(settings.InitialScatterTimeInSeconds))
                       .Then(context => Actions.ScatterGhosts(context.Instance)));

            During(Scatter,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .TransitionTo(GhostChase));

            During(ChangingLevel,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .TransitionTo(Scatter));

            WhenLeave(ChangingLevel, binder => binder.Then(context => Actions.GetReadyForNextLevel(context.Instance, settings)));
            
            WhenEnter(GhostChase,
                       binder => binder
                            .Then(context => context.Instance.ChangeStateIn(settings.ChaseTimeInSeconds))
                            .Then(context => Actions.GhostToChase(context.Instance)));

            During(GhostChase,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .TransitionTo(Scatter));

            During(Frightened,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => Actions.MakeGhostsNotEdible(context.Instance))
                    .TransitionTo(Scatter));

            During(Scatter, GhostChase, Frightened,
                When (PlayersWishesToChangeDirection)
                    .Then(context => Actions.ChangeDirection(game, context.Instance, context.Data.NewDirection)),
                When(Tick)
                    .ThenAsync(async context => await Actions.MoveGhosts(game, context.Instance, context, this))
                    .ThenAsync(async context => await Actions.MovePacMan(game, context.Instance, context, this, settings)),
                When(CoinCollision)
                    .Then(context => Actions.CoinEaten(game, settings, context.Instance, context.Data.Location, gameNotifications))
                    .If(context => context.Instance.IsLevelComplete(), 
                            binder => binder.TransitionTo(ChangingLevel)),
                When(FruitCollision)
                    .Then(context => Actions.FruitEaten(game, settings, context.Instance, context.Data.Location, gameNotifications)),
                When(PowerPillCollision)
                    .Then(context => Actions.PowerPillEaten(settings, context.Instance, context.Data.Location, gameNotifications))
                    .IfElse(context => context.Instance.IsLevelComplete(), 
                            binder => binder.TransitionTo(ChangingLevel),
                        binder =>
                            binder.Then(context => context.Instance.ChangeStateIn(settings.FrightenedTimeInSeconds))
                                .TransitionTo(Frightened)),
                When(GhostCollision)
                    .IfElse(x => x.Data.Ghost.Edible,
                    binder => binder.Then(context => Actions.GhostEaten(context.Instance, context.Data.Ghost, game, gameNotifications)),
                    binder => binder.Then(context => Actions.EatenByGhost(context.Instance))
                                    .TransitionTo(Dying))); 


            WhenEnter(ChangingLevel,
                       binder => binder
                                .Then(context => context.Instance.HideGhosts())
                                .Then(context => context.Instance.ChangeStateIn(4)));


            WhenEnter(Dying,
                       binder => binder
                                .Then(context => Actions.BeginDying(context.Instance, gameNotifications))
                                .Then(context => context.Instance.ChangeStateIn(4)));

            During(Dying,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .IfElse(context => context.Instance.Lives > 0,
                        binder => binder.TransitionTo(Respawning),
                        binder => binder.TransitionTo(Dead)));

            WhenEnter(Respawning,
                       binder => binder
                                .Then(context => context.Instance.ChangeStateIn(4))
                                .Then(context => Actions.BeginRespawning(gameNotifications)));

            During(Respawning,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => Actions.CompleteRespawning(context.Instance, settings))
                    .TransitionTo(Scatter));

            During(Dead, Ignore(Tick));

            During(Dying, Respawning, Dead, ChangingLevel,
                    Ignore(PlayersWishesToChangeDirection),
                    Ignore(CoinCollision),
                    Ignore(PowerPillCollision),
                    Ignore(GhostCollision));
        }

        public State GhostChase { get; private set; } = null!;
        public State Scatter { get; private set; } = null!;
        public State Frightened { get; private set; } = null!;
        public State Dying { get; private set; } = null!;
        public State Respawning { get; private set; } = null!;
        public State Dead { get; private set; } = null!;
        public State ChangingLevel { get; private set; } = null!;
        public Event<Tick> Tick { get; private set; } = null!;
        public Event<GhostCollision> GhostCollision { get; private set; } = null!;
        public Event<CoinCollision> CoinCollision { get; private set; } = null!;
        public Event<PowerPillCollision> PowerPillCollision { get; private set; } = null!;
        public Event<PlayersWishesToChangeDirection> PlayersWishesToChangeDirection { get; private set; } = null!;
        public Event<FruitCollision> FruitCollision { get; private set; } = null!;
    }
}