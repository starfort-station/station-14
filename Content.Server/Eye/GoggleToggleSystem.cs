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
    public override void Initialize()
    {
        base.Initialize();

        //SubscribeLocalEvent<GoggleToggleComponent, GotEquippedEvent>(OnGotEquipped);
        //SubscribeLocalEvent<GoggleToggleComponent, GotUnequippedEvent>(OnGotUnequipped);
    }

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

    //private void OnGotUnequipped(EntityUid uid, GoggleToggleComponent component, GotUnequippedEvent args)
    //{
    //    if (args.Slot == "eyes")
    //    {
    //        if (TryComp<DarkVisionComponent>(args.Equipee, out var vision))
    //        {
    //            vision.IsEnable = component.On = false;
    //            vision.DrawLight = true;
    //            Dirty(vision);
    //        }
    //    }
    //}

    //private void OnGotEquipped(EntityUid uid, GoggleToggleComponent component, GotEquippedEvent args)
    //{
    //    if (args.Slot == "eyes")
    //    {
    //        if (TryComp<DarkVisionComponent>(args.Equipee, out var vision))
    //        {
    //            vision.IsEnable = component.On;
    //            vision.DrawLight = component.DrawLight;
    //            Dirty(vision);
    //        }
    //    }
    //}
}
