using Content.Shared.Eye;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;

namespace Content.Client.Eye;
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

    private DarkVisionComponent? _darkVisionComponent = default!;

    public DarkVisionOverlay()
    {
        IoCManager.InjectDependencies(this);

        if (_darkVisionComponent?.ShaderTexturePrototype == null)
        {
            _darkShader = _prototypeManager.Index<ShaderPrototype>("NightVisionRoboto").InstanceUnique();
            return;
        }

        _darkShader = _prototypeManager.Index<ShaderPrototype>(
                _darkVisionComponent.ShaderTexturePrototype).InstanceUnique();
    }

    public void SetShaderProto(String proto)
    {
        _darkShader = _prototypeManager.Index<ShaderPrototype>(proto).InstanceUnique();
    }


    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        var playerEntity = _playerManager.LocalPlayer?.ControlledEntity;

        if (playerEntity == null)
            return false;

        var darkVision = _entityManager.GetComponent<DarkVisionComponent>(playerEntity.Value);
        if (darkVision == null || !darkVision.IsEnable)
            return false;
        
        _darkVisionComponent = darkVision;
        return true;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null || _darkVisionComponent == null)
            return;

        _darkShader?.SetParameter("SCREEN_TEXTURE", ScreenTexture);

        var worldHandle = args.WorldHandle;

        _lightManager.DrawLighting = _darkVisionComponent.DrawLight;
        worldHandle.UseShader(_darkShader);
        worldHandle.DrawRect(args.WorldBounds, _darkVisionComponent.LayerColor);
        worldHandle.UseShader(null);
    }
}
