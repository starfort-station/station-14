
using Content.Shared.Eye.Blinding;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Eye.Blinding;
using Content.Shared.Administration;
using Content.Shared.Administration.Events;
using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;

namespace Content.Client.Eye.DarkVision;

public sealed class DarkVisionSystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly ILightManager _lightManager = default!;


    private DarkVisionOverlay _darkOverlay = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DarkVisionComponent, ComponentInit>(OnVisionInit);
        SubscribeLocalEvent<DarkVisionComponent, ComponentShutdown>(OnVisionShutdown);

        SubscribeLocalEvent<DarkVisionComponent, PlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<DarkVisionComponent, PlayerDetachedEvent>(OnPlayerDetached);

        SubscribeNetworkEvent<RoundRestartCleanupEvent>(RoundRestartCleanup);

        _darkOverlay = new DarkVisionOverlay();
    }

    private void OnPlayerAttached(EntityUid uid, DarkVisionComponent component, PlayerAttachedEvent args)
    {
        if (component.ShaderTexturePrototype != null)
        {
            _darkOverlay.SetShaderProto(component.ShaderTexturePrototype);
        }
        _lightManager.DrawLighting = component.DrawLight;
        _overlayMan.AddOverlay(_darkOverlay);
        
    }

    private void OnPlayerDetached(EntityUid uid, DarkVisionComponent component, PlayerDetachedEvent args)
    {
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
        if (_player.LocalPlayer?.ControlledEntity == uid)
        {
            _lightManager.DrawLighting = true;
            _overlayMan.RemoveOverlay(_darkOverlay);
        }
    }

    private void RoundRestartCleanup(RoundRestartCleanupEvent ev)
    {
        _lightManager.Enabled = true;
    }
}
