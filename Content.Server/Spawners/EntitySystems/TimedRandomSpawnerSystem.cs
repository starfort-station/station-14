using Robust.Shared.Timing;
using Content.Server.Spawners.Components;
using Content.Server.Destructible;
using Content.Server.Stack;
using System.Threading;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Content.Shared.Mobs.Systems;
using Content.Shared.Spawners.Components;

namespace Content.Server.Spawners.EntitySystems
{
    public class TimedRandomSpawnerSystem : EntitySystem
    {
        [Dependency] private readonly IGameTiming _time = default!;
        [Dependency] private readonly IRobustRandom _robustRandom = default!;
        [Dependency] public readonly IPrototypeManager PrototypeManager = default!;
        [Dependency] public readonly IComponentFactory ComponentFactory = default!;
        [Dependency] public readonly StackSystem StackSystem = default!;

        //[Dependency] public readonly IEntityManager EntityManager = default!;
        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<TimedRandomSpawnerComponent, ComponentStartup>(OnStartup);
            SubscribeLocalEvent<TimedRandomSpawnerComponent, TimerReached>(Exec);
        }

        private void Exec(EntityUid owner, TimedRandomSpawnerComponent component, TimerReached args)
        {
            if (!_robustRandom.Prob(component.Chance))
                return;

            var number = _robustRandom.Next(component.MinimumEntitiesSpawned, component.MaximumEntitiesSpawned);
            for (int i = 0; i < number; i++)
            {
                var entity = _robustRandom.Pick(component.Prototypes);
                var xform = Transform(owner);
                var spawned = Spawn(entity, xform.Coordinates.Offset(_robustRandom.NextVector2(0.3f)));
                var spawnedEvent = new NewEntitySpawned();
                spawnedEvent.parent = owner;
                spawnedEvent.spawned = spawned;
                RaiseLocalEvent(owner, spawnedEvent);
            }

        }


        public CancellationTokenSource? TokenSource;
        private void OnStartup(EntityUid uid, TimedRandomSpawnerComponent component, ComponentStartup args)
        {
            SetupTimer(uid, component);
        }
        private void SetupTimer(EntityUid owner, TimedRandomSpawnerComponent component)
        {
            component.LastActivationTime = _time.CurTime;
        }

        public sealed class TimerReached : EntityEventArgs
        {

        }
        public sealed class NewEntitySpawned : EntityEventArgs
        {
            public EntityUid parent;
            public EntityUid spawned;
        }
        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            List<TimedRandomSpawnerComponent> toUpdate = new();
            foreach (var comp in EntityQuery<TimedRandomSpawnerComponent>())
            {
                var timeDif = _time.CurTime - comp.LastActivationTime;
                if (timeDif <= comp.IntervalSeconds)
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
