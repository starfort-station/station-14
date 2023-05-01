using Robust.Client.Graphics;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Client.Eye.DarkVision
{
    [RegisterComponent]
    [NetworkedComponent]
    public sealed class DarkVisionComponent : Component
    {
        [DataField("shaderTexture")]
        public String? ShaderTexturePrototype;

        [DataField("layerColor")]
        public Color LayerColor = Color.FromHex("#00000000");

        [DataField("mustDrawLight")]
        public bool DrawLight = false;
    }
}
