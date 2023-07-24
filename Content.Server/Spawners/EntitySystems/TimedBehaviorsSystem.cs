using Robust.Shared.Timing;
using Content.Server.Spawners.Components;
using Content.Server.Destructible;
using Content.Server.Spawners.Components;
using Robust.Shared.Timing;
using Content.Shared.Stacks;
using Content.Server.Stack;
using System.Threading;
using Content.Shared.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.GameObjects;
using System.Numerics;

namespace Content.Server.Spawners.EntitySystems
{
    public class TimedBehaviorsSystem : EntitySystem
    {
        [Dependency] private readonly IGameTiming _time = default!;
        [Dependency] public readonly IRobustRandom Random = default!;
        [Dependency] public readonly IPrototypeManager PrototypeManager = default!;
        [Dependency] public readonly IComponentFactory ComponentFactory = default!;
        [Dependency] public readonly StackSystem StackSystem = default!;
        [Dependency] public readonly DestructibleSystem DestructibleSystem = default!;
        [Dependency] public readonly IEntityManager EntityManager = default!;
        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<TimedBehaviorsComponent, ComponentStartup>(OnStartup);
            SubscribeLocalEvent<TimedBehaviorsComponent, TimerReached>(Exec);
        }

        private void Exec2(EntityUid owner, TimedBehaviorsComponent component, TimerCallback args)
        {
            var timeDif = _time.CurTime - component.LastActivationTime;
            if (timeDif <= component.ActivationRate)
                return;
            component.LastActivationTime = _time.CurTime;
            RaiseLocalEvent(owner, new TimerReached(), true);
            foreach (var behavior in component.Behaviors)
            {
                // The owner has been deleted. We stop execution of behaviors here.
                if (!EntityManager.EntityExists(owner))
                    return;

                behavior.Execute(owner, DestructibleSystem);
            }
        }

        private void Exec(EntityUid owner, TimedBehaviorsComponent component, TimerReached args)
        {

            foreach (var behavior in component.Behaviors)
            {
                // The owner has been deleted. We stop execution of behaviors here.
                if (!EntityManager.EntityExists(owner))
                    return;

                behavior.Execute(owner, DestructibleSystem);
            }
        }


        public CancellationTokenSource? TokenSource;
        private void OnStartup(EntityUid uid, TimedBehaviorsComponent component, ComponentStartup args)
        {
            SetupTimer(uid, component);
        }
        private void SetupTimer(EntityUid owner, TimedBehaviorsComponent component)
        {
            component.LastActivationTime = _time.CurTime;
        }

        public sealed class TimerReached : EntityEventArgs
        {

        }
        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            List<TimedBehaviorsComponent> toUpdate = new();
            foreach (var  comp in EntityQuery<TimedBehaviorsComponent>())
            {
                var timeDif = _time.CurTime - comp.LastActivationTime;
                if (timeDif <= comp.ActivationRate)
                    continue;

                toUpdate.Add(comp);
                comp.LastActivationTime = _time.CurTime;
            }

            foreach (var a in toUpdate)
            {
            RaiseLocalEvent(a.Owner, new TimerReached(), true);
            }
        }

    }

}
