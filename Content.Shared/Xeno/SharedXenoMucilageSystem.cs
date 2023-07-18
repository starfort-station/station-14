using System.Linq;
using Content.Shared.Xeno;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Xeno;

public abstract class SharedXenoMucilageSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _action = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoMucilageObjectComponent, ComponentStartup>(OnXenoMucilageStartup);
        SubscribeLocalEvent<XenoMucilageComponent, ComponentStartup>(OnXenoMucilageStartup);
    }

    private void OnXenoMucilageStartup(EntityUid uid, XenoMucilageComponent component, ComponentStartup args)
    {
        var netAction = new InstantAction(_proto.Index<InstantActionPrototype>(component.WebActionName));
        _action.AddAction(uid, netAction, null);
    }

    private void OnXenoMucilageStartup(EntityUid uid, XenoMucilageObjectComponent component, ComponentStartup args)
    {
        _appearance.SetData(uid, XenoMucilageVisuals.Variant, _robustRandom.Next(1, 3));
    }
}
