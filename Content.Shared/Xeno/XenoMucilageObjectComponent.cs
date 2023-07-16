using Robust.Shared.GameStates;

namespace Content.Shared.Xeno;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedXenoMucilageSystem))]
public sealed class XenoMucilageObjectComponent : Component
{
}
