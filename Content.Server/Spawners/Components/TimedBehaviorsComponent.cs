using System.Threading;
using Content.Server.Destructible.Thresholds;
using System.Numerics;
using Content.Server.Forensics;
using Content.Server.Stack;
using Content.Shared.Prototypes;
using Content.Shared.Stacks;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.Timing;
using Robust.Shared.GameObjects;
using Content.Server.Destructible.Thresholds.Behaviors;

namespace Content.Server.Spawners.Components
{

    [RegisterComponent]
    public sealed class TimedBehaviorsComponent : Component, ISerializationHooks
    {
        [DataField("behaviors")]
        private List<IThresholdBehavior> _behaviors = new();

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("intervalSeconds")]
        public TimeSpan ActivationRate { get; set; } = TimeSpan.FromSeconds(60);

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("LastActivation")]
        public TimeSpan LastActivationTime;
        [ViewVariables] public IReadOnlyList<IThresholdBehavior> Behaviors => _behaviors;

    }
}
