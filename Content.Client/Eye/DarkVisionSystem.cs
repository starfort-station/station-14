using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Content.Shared.Clothing.Components;
using Content.Shared.Eye.DarkVision;

namespace Content.Client.Eye.DarkVision;

public sealed class DarkVisionSystem : DarkVisionSharedSystem
{
    [Dependency] private readonly IPlayerManager _player = default!;

    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly ILightManager _lightManager = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;


    private DarkVisionOverlay _darkOverlay = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DarkVisionComponent, ComponentInit>(OnVisionInit);
        SubscribeLocalEvent<DarkVisionComponent, ComponentShutdown>(OnVisionShutdown);

        SubscribeLocalEvent<DarkVisionComponent, PlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<DarkVisionComponent, PlayerDetachedEvent>(OnPlayerDetached);

        _darkOverlay = new DarkVisionOverlay();
    }

    public override void ForceUpdate(EntityUid uid, DarkVisionComponent component)
    {
        if (_player.LocalPlayer?.ControlledEntity == uid)
        {
            if (component.IsEnable)
            {
                if (component.ShaderTexturePrototype != null)
                {
                    _darkOverlay.SetShaderProto(component.ShaderTexturePrototype);
                }
                _lightManager.DrawLighting = component.DrawLight;
                _overlayMan.AddOverlay(_darkOverlay);
            }
            else
            {
                _overlayMan.RemoveOverlay(_darkOverlay);
                _lightManager.DrawLighting = true;
            }
        }
    }

    private void OnPlayerAttached(EntityUid uid, DarkVisionComponent component, PlayerAttachedEvent args)
    {
        if (!component.IsEnable)
            return;

        if (component.ShaderTexturePrototype != null)
        {
            _darkOverlay.SetShaderProto(component.ShaderTexturePrototype);
        }
        _lightManager.DrawLighting = component.DrawLight;
        _overlayMan.AddOverlay(_darkOverlay);
        
    }

    private void OnPlayerDetached(EntityUid uid, DarkVisionComponent component, PlayerDetachedEvent args)
    {
        if (!component.IsEnable)
            return;

        _overlayMan.RemoveOverlay(_darkOverlay);
        _lightManager.DrawLighting = true;
    }

    private void OnVisionInit(EntityUid uid, DarkVisionComponent component, ComponentInit args)
    {
        if (_player.LocalPlayer?.ControlledEntity == uid)
        {
            _lightManager.DrawLighting = component.DrawLight;
            _overlayMan.AddOverlay(_darkOverlay);
        }
    }

    private void OnVisionShutdown(EntityUid uid, DarkVisionComponent component, ComponentShutdown args)
    {
        if (!component.IsEnable)
            return;

        if (_player.LocalPlayer?.ControlledEntity == uid)
        {
            _lightManager.DrawLighting = true;
            _overlayMan.RemoveOverlay(_darkOverlay);
        }
    }
}
