using Robust.Shared.GameStates;

namespace Content.Shared.Eye
{
    [RegisterComponent]
    [NetworkedComponent]
    public sealed partial class DarkVisionComponent : Component
    {
        [DataField("shaderTexture")]
        public String? ShaderTexturePrototype;

        [DataField("layerColor")]
        public Color LayerColor = Color.FromHex("#00000000");

        [DataField("mustDrawLight")]
        public bool DrawLight = true;

        [DataField("toggle")]
        public bool IsEnable = false;
    }
}
