using Content.Server.Body.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Timer = Robust.Shared.Timing.Timer;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System;
using System.Linq;
using System.Threading;
using Content.Shared.IdentityManagement;


namespace Content.Server.Body.Systems
{
    /// <summary>
    /// Assigns a loadout to an entity based on the startingGear prototype
    /// </summary>
    public sealed class RandomDamageSystem : EntitySystem
    {
        [Dependency] private readonly DamageableSystem _damageableSystem = default!;
        [Dependency] private readonly IPrototypeManager _protoMan = default!;
        [Dependency] private readonly IRobustRandom _random = default!;

        public List<timerEventHandle> allPresenterEventHandles = new();

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<RandomDamageComponent, ComponentStartup>(OnStartup);
        }

        private void OnStartup(EntityUid uid, RandomDamageComponent component, ComponentStartup args)
        {
            if (TryComp<DamageableComponent>(uid, out var damageComp))
            {
                // @todo refactor, must find all necessary proto in 1 loop
                // TryIdex get proto in every loop when it used
                List<DamageTypePrototype> randDamageTypes = new(); //Heat Blunt Piercing Slash Asphyxiation Poison Bloodloss
                {
                    if (_protoMan.TryIndex<DamageTypePrototype>("Heat", out var spec))
                    {
                        randDamageTypes.Add(spec);
                    }
                }
                {
                    if (_protoMan.TryIndex<DamageTypePrototype>("Blunt", out var spec))
                    {
                        randDamageTypes.Add(spec);
                    }
                }
                {
                    if (_protoMan.TryIndex<DamageTypePrototype>("Piercing", out var spec))
                    {
                        randDamageTypes.Add(spec);
                    }
                }
                {
                    if (_protoMan.TryIndex<DamageTypePrototype>("Slash", out var spec))
                    {
                        randDamageTypes.Add(spec);
                    }
                }
                {
                    if (_protoMan.TryIndex<DamageTypePrototype>("Bloodloss", out var spec))
                    {
                        randDamageTypes.Add(spec);
                    }
                }
                if (randDamageTypes == null || !randDamageTypes.Any())
                    return;

                allPresenterEventHandles.Add(new timerEventHandle(uid, randDamageTypes,damageComp,_damageableSystem,_random, this));
            }
        }
    }

    public class timerEventHandle
    {
        private CancellationTokenSource _timerCancelTokenSource = new();
        private EntityUid _entity;
        private List<DamageTypePrototype> _randDamageTypes;
        private DamageableComponent _damageableComponent;
        private DamageableSystem _damageableSystem;
        private IRobustRandom _random;
        private RandomDamageSystem _ownerSystem;
        public timerEventHandle(EntityUid entity, List<DamageTypePrototype> randDamageTypes,
            DamageableComponent damageableComponent, DamageableSystem damageableSystem,
            IRobustRandom random, RandomDamageSystem ownerSystem)
        {
            _entity = entity;
            _randDamageTypes = randDamageTypes;
            _damageableComponent = damageableComponent;
            _timerCancelTokenSource.Cancel();
            _timerCancelTokenSource = new CancellationTokenSource();
            _damageableSystem = damageableSystem;
            _random = random;
            _ownerSystem = ownerSystem;

            Timer.Spawn(3000, () =>
            {
                float maxDamage = _random.NextFloat(210, 360);

                if (randDamageTypes.Count() == 1)
                {
                    _damageableSystem.TryChangeDamage(_entity, new DamageSpecifier(randDamageTypes[0], maxDamage));
                }
                else
                {
                    float gettedDamage = 0;

                    for (int index = 1; index < randDamageTypes.Count(); ++index)
                    {
                        if (gettedDamage > maxDamage)
                            break;
                        float firstDamage = _random.NextFloat(20, 80);
                        _damageableSystem.TryChangeDamage(_entity, new DamageSpecifier(randDamageTypes[index], firstDamage));
                        gettedDamage += firstDamage;
                    }

                    if (gettedDamage < maxDamage)
                    {
                        float lastDamage = maxDamage - gettedDamage;
                        _damageableSystem.TryChangeDamage(_entity, new DamageSpecifier(randDamageTypes[1], lastDamage));
                    }
                }

                _ownerSystem.allPresenterEventHandles.Remove(this);
            }, _timerCancelTokenSource.Token);
        }

    }
}
