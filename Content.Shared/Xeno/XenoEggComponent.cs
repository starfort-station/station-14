using System.Threading;
using Content.Shared.Actions;
using Robust.Shared.GameStates;

namespace Content.Shared.Xeno;

    [RegisterComponent, NetworkedComponent]
    [Access(typeof(SharedXenoEggSystem))]
    public sealed class XenoEggComponent : Component
    {

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("prototype")]
        public string Prototype { get; set; } = string.Empty;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("MinimumEntitiesSpawned")]
        public int MinimumEntitiesSpawned { get; set; } = 1;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("MaximumEntitiesSpawned")]
        public int MaximumEntitiesSpawned { get; set; } = 1;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("Action")]
        public string action = "XenoEggAction";
    }

public sealed class XenoEggActionEvent : InstantActionEvent { }
