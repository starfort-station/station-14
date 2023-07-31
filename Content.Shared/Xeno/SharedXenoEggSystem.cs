using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Prototypes;

namespace Content.Shared.Xeno;

public abstract class SharedXenoEggSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _action = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoEggComponent, ComponentStartup>(OnXenoEggStartup);
    }

    private void OnXenoEggStartup(EntityUid uid, XenoEggComponent component, ComponentStartup args)
    {
        var netAction = new InstantAction(_proto.Index<InstantActionPrototype>(component.action));
        _action.AddAction(uid, netAction, null);
    }
}
