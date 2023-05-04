using Content.Server.Eye.DarkVision;
using Content.Shared.Actions;
using Content.Shared.Alert;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Eye.DarkVision;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;

namespace Content.Server.Clothing;

public sealed class GoggleToggleSystem : GoggleToggleSharedSystem
{
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly DarkVisionSystem _darkVision = default!;

    protected override void UpdateGogglesState(EntityUid uid, GoggleToggleComponent goggles)
    {
        if (TryComp<DarkVisionComponent>(uid, out var vision))
        {
            vision.DrawLight = goggles.DrawLight;
            vision.LayerColor = goggles.LayerColor;
            vision.ShaderTexturePrototype = goggles.ShaderTexturePrototype;
            vision.IsEnable = goggles.On;
        }
        else
        {
            var visionComponent = new DarkVisionComponent
            {
                IsEnable = goggles.On,
                DrawLight = goggles.DrawLight,
                LayerColor = goggles.LayerColor,
                ShaderTexturePrototype = goggles.ShaderTexturePrototype,
                Owner = uid
            };
            _entManager.AddComponent(uid, visionComponent, true);
        }
    }

}
