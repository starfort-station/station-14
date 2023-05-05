using Robust.Client.Graphics;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Eye.DarkVision
{
    [RegisterComponent]
    [AutoGenerateComponentState]
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
        [AutoNetworkedField]
        public bool IsEnable = false;
    }
}
