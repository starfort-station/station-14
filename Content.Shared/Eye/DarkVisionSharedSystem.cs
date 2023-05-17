using Robust.Shared.Serialization;

namespace Content.Shared.Eye;

[Serializable, NetSerializable]
public sealed class RequestUpdateOverlayMessage : EntityEventArgs
{
    public EntityUid Id { get; }
    public String ShaderTexturePrototype { get; }
    public Color LayerColor { get; }
    public bool DrawLight { get; }
    public bool On { get; }
    public RequestUpdateOverlayMessage(EntityUid id, string shaderTexture,
        Color layerColor, bool drawLight, bool toggleOn)
    {
        Id = id;
        ShaderTexturePrototype = shaderTexture;
        LayerColor = layerColor;
        DrawLight = drawLight;
        On = toggleOn;
    }
}

public abstract class DarkVisionSharedSystem : EntitySystem
{
    // Maybe must be deleted
}
