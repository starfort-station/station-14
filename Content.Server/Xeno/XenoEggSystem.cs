using Content.Server.Popups;
using Content.Shared.Xeno;
using Robust.Shared.Random;
namespace Content.Server.Xeno;

public sealed class XenoEggSystem : SharedXenoEggSystem
{
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<XenoEggComponent, XenoEggActionEvent>(OnSpawnEgg);
    }

    private void OnSpawnEgg(EntityUid uid, XenoEggComponent component, XenoEggActionEvent args)
    {
        if (args.Handled)
            return;
        var number = _robustRandom.Next(component.MinimumEntitiesSpawned, component.MaximumEntitiesSpawned + 1);
        var xform = Transform(uid);
        for (var i = 0; i < number; i++)
        {

            Spawn(component.Prototype, xform.Coordinates.Offset(_robustRandom.NextVector2(0.6f)));
            _popup.PopupEntity(Loc.GetString("xeno-egg-action-success"), args.Performer, args.Performer);
            args.Handled = true;
        }
    }
}

