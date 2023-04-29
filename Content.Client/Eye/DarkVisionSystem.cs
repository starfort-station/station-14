
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
    [Dependency] ILightManager _lightManager = default!;


    private DarkVisionOverlay _overlay = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DarkVisionComponent, ComponentInit>(OnVisionInit);
        SubscribeLocalEvent<DarkVisionComponent, ComponentShutdown>(OnVisionShutdown);

        SubscribeLocalEvent<DarkVisionComponent, PlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<DarkVisionComponent, PlayerDetachedEvent>(OnPlayerDetached);

        SubscribeNetworkEvent<RoundRestartCleanupEvent>(RoundRestartCleanup);

        _overlay = new();
    }

    private void OnPlayerAttached(EntityUid uid, DarkVisionComponent component, PlayerAttachedEvent args)
    {
        _overlayMan.AddOverlay(_overlay);
    }

    private void OnPlayerDetached(EntityUid uid, DarkVisionComponent component, PlayerDetachedEvent args)
    {
        _overlayMan.RemoveOverlay(_overlay);
        _lightManager.DrawLighting = true;
    }

    private void OnVisionInit(EntityUid uid, DarkVisionComponent component, ComponentInit args)
    {
        if (_player.LocalPlayer?.ControlledEntity == uid)
            _overlayMan.AddOverlay(_overlay);
    }

    private void OnVisionShutdown(EntityUid uid, DarkVisionComponent component, ComponentShutdown args)
    {
        if (_player.LocalPlayer?.ControlledEntity == uid)
        {
            _overlayMan.RemoveOverlay(_overlay);
        }
    }

    private void RoundRestartCleanup(RoundRestartCleanupEvent ev)
    {
        _lightManager.Enabled = true;
    }
}
