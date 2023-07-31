using Robust.Shared.Timing;
using Content.Server.Spawners.Components;
using Content.Server.Destructible;
using Content.Server.Stack;
using System.Threading;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Content.Shared.Mobs.Systems;
using Content.Shared.Spawners.Components;
using static Content.Server.Spawners.EntitySystems.TimedBehaviorsSystem;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using static Content.Server.Spawners.EntitySystems.TimedRandomSpawnerSystem;
using Content.Server.Xeno;

namespace Content.Server.Spawners.EntitySystems
{
    public class XenoLarvaSystem : EntitySystem
    {
        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<XenoLarvaComponent, NewEntitySpawned>(ChangeMind);
            SubscribeLocalEvent<XenoLarvaComponent, UpdateMobStateEvent>(DisableSpawn);
        }

        private void DisableSpawn(EntityUid uid, XenoLarvaComponent component, ref UpdateMobStateEvent args)
        {
            var timedRandomSpawnerComponent = EntityManager.GetComponent<TimedRandomSpawnerComponent>(uid);
            RemCompDeferred(uid, timedRandomSpawnerComponent);
            var despawnComponent = EntityManager.GetComponent<TimedDespawnComponent>(uid);
            RemCompDeferred(uid, despawnComponent);
        }

        private void ChangeMind(EntityUid owner, XenoLarvaComponent component, NewEntitySpawned args)
        {
            if(HasComp<MindContainerComponent>(owner)){
                var mindSystem = EntityManager.System<MindSystem>();

                var playerMind = mindSystem.GetMind(owner);
                if (playerMind != null){
                    mindSystem.TransferTo(playerMind, args.spawned, true);
                }
            }

        }


    }

}
