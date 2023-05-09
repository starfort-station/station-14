using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Eye;

namespace Content.Server.Eye;

public sealed class GoggleToggleSystem : GoggleToggleSharedSystem
{
    [Dependency] private readonly IEntityManager _entManager = default!;

    protected override void UpdateGogglesState(EntityUid uid, GoggleToggleComponent goggles)
    {
        DarkVisionComponent tempComp;

        if (TryComp<DarkVisionComponent>(uid, out var vision))
        {
            vision.DrawLight = goggles.DrawLight;
            vision.LayerColor = goggles.LayerColor;
            vision.ShaderTexturePrototype = goggles.ShaderTexturePrototype;
            vision.IsEnable = goggles.On;

            tempComp = vision;
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
            tempComp = visionComponent;

        }
        if (tempComp.ShaderTexturePrototype == null)
            return;

        RaiseNetworkEvent(new RequestUpdateOverlayMessage(uid, tempComp.ShaderTexturePrototype,
            tempComp.LayerColor, tempComp.DrawLight, tempComp.IsEnable));
    }

}
