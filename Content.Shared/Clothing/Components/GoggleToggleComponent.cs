using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Clothing.EntitySystems;
using Robust.Shared.GameStates;

namespace Content.Shared.Clothing.Components
{
    [Access(typeof(GoggleToggleSharedSystem))]
    [RegisterComponent]
    [NetworkedComponent]
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

        // only this field will had changes in runtime
        [DataField("on")]
        [AutoNetworkedField]
        public bool On;
    }

    public sealed class ToggleGogglesEvent : InstantActionEvent { }
}
