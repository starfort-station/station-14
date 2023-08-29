using Content.Shared.Damage;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Medical.Dropper;

[RegisterComponent, NetworkedComponent]
public sealed class DropperConnectableComponent : Component
{
    public EntityUid? dropper = null;
}



[ByRefEvent]
public readonly record struct DropperNeedleRemovedEvent(EntityUid Target)
{
    public readonly EntityUid Target = Target;
}

