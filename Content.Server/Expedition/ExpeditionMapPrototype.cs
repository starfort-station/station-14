using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Expedition
{
    [Prototype("expeditionMap")]
    public sealed class ExpeditionMapPrototype : IPrototype
    {
        [ViewVariables]
        [IdDataField]
        public string ID { get; } = default!;

        /// <summary>
        /// Relative directory path to the given map, i.e. `Maps/Expedition/template.yml`
        /// </summary>
        [DataField("mapPath", required: true)]
        public ResPath MapPath { get; } = default!;

        /// <summary>
        /// Name for admin use
        /// </summary>
        [DataField("name")]
        public string Name { get; } = "";

        /// <summary>
        /// Visible name of FTL point
        /// </summary>
        [DataField("ftlName", required: true)]
        public string FTLName { get; } = "strange signal";
    }
}
