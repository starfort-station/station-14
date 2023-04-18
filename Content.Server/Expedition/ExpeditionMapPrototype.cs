using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Server.Expedition
{
    [Prototype("expeditionMap")]
    public sealed class ExpeditionMapPrototype : IPrototype
    {
        [ViewVariables]
        [IdDataField]
        public string ID { get; } = default!;

        /// <summary>
        /// Relative directory path to the given map, i.e. `Maps/Salvage/template.yml`
        /// </summary>
        [DataField("mapPath", required: true)]
        public ResourcePath MapPath { get; } = default!;


        /// <summary>
        /// Name for admin use
        /// </summary>
        [DataField("name")]
        public string Name { get; } = "";
    }
}
