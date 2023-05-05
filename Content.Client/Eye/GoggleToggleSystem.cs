using Content.Client.Eye.DarkVision;
using Content.Shared.Actions;
using Content.Shared.Alert;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Eye.DarkVision;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;

namespace Content.Client.Clothing;

public sealed class GoggleToggleSystem : GoggleToggleSharedSystem
{
    [Dependency] private readonly DarkVisionSystem _darkVision = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;
    protected override void UpdateGogglesState(EntityUid uid, GoggleToggleComponent goggles)
    {
        if (TryComp<DarkVisionComponent>(uid, out var vision))
        {
            vision.DrawLight = goggles.DrawLight;
            vision.LayerColor = goggles.LayerColor;
            vision.ShaderTexturePrototype = goggles.ShaderTexturePrototype;
            vision.IsEnable = goggles.On;
            _darkVision.ForceUpdate(uid, vision);
        }
    }
}
