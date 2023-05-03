using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Eye.DarkVision;
using Robust.Shared.GameStates;

namespace Content.Shared.Clothing.Components
{
    [Access(typeof(GoggleToggleSharedSystem))]//,typeof(DarkVisionSharedSystem))]
    [RegisterComponent]
    [AutoGenerateComponentState]
    public sealed partial class GoggleToggleComponent : Component
    {
        [DataField("toggleAction")]
        public InstantAction? ToggleAction = null;

        [DataField("shaderTexture")]
        public String? ShaderTexturePrototype;

        [DataField("layerColor")]
        public Color LayerColor = Color.FromHex("#00000000");

        [DataField("mustDrawLight")]
        public bool DrawLight = false;

        [DataField("on"), AutoNetworkedField]
        public bool On;
    }

    public sealed class ToggleGogglesEvent : InstantActionEvent { }
}
