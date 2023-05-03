using Content.Client.Drugs;
using Content.Shared.Drugs;
using Content.Shared.Eye.Blinding;
using Content.Shared.StatusEffect;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using KeyNotFoundException = System.Collections.Generic.KeyNotFoundException;


namespace Content.Client.Eye.DarkVision;
public sealed class DarkVisionOverlay : Overlay
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IEntitySystemManager _sysMan = default!;
    [Dependency] IEntityManager _entityManager = default!;
    [Dependency] ILightManager _lightManager = default!;


    public override bool RequestScreenTexture => true;
    public override OverlaySpace Space => OverlaySpace.WorldSpace;

    private ShaderInstance _darkShader;
    //private readonly ShaderInstance _circleMaskShader;

    private DarkVisionComponent? _darkVisionComponent = default!;
    private float _timeDelta = 0;

    public DarkVisionOverlay()
    {
        IoCManager.InjectDependencies(this);
        if (_darkVisionComponent == null)
        {
            _darkShader = _prototypeManager.Index<ShaderPrototype>("NightVisionRoboto").InstanceUnique();
            return;
        }

        if (_darkVisionComponent.ShaderTexturePrototype == null)
        {
            _darkShader = _prototypeManager.Index<ShaderPrototype>("NightVisionRoboto").InstanceUnique();
            return;
        }

        _darkShader = _prototypeManager.Index<ShaderPrototype>(_darkVisionComponent.ShaderTexturePrototype)
                .InstanceUnique();
    }

    public void SetShaderProto(String proto)
    {
        _darkShader = _prototypeManager.Index<ShaderPrototype>(proto).InstanceUnique();
    }


    protected override void FrameUpdate(FrameEventArgs args)
    {


        _timeDelta = args.DeltaSeconds;
    }

    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        var playerEntity = _playerManager.LocalPlayer?.ControlledEntity;

        if (playerEntity == null)
            return false;

        var darkVision = _entityManager.GetComponent<DarkVisionComponent>(playerEntity.Value);
        if (darkVision == null)
            return false;
        _darkVisionComponent = darkVision;
        return true;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null || _darkVisionComponent == null)
            return;
        if (_timeDelta == null)
            return;

        //_lightManager.DrawHardFov = true;
        _lightManager.DrawLighting = false;
        //_lightManager.DrawShadows = false;

        //_darkShader?.SetParameter("DELTA_TIME", _timeDelta);
        _darkShader?.SetParameter("SCREEN_TEXTURE", ScreenTexture);

        var worldHandle = args.WorldHandle;
        worldHandle.UseShader(_darkShader);
        worldHandle.DrawRect(args.WorldBounds, _darkVisionComponent.LayerColor);
        worldHandle.UseShader(null);
    }
}
