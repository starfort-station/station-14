using Content.Shared.Actions;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Xeno;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedXenoMucilageSystem))]
public sealed class XenoMucilageComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("webPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string WebPrototype = "XenoMucilage";

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("webActionName")]
    public string WebActionName = "XenoMucilageAction";
}

public sealed class XenoMucilageActionEvent : InstantActionEvent { }
