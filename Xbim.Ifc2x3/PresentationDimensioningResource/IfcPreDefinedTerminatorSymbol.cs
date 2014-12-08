using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.PresentationDefinitionResource;
using Xbim.XbimExtensions;

namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
    [IfcPersistedEntity]
    public class IfcPreDefinedTerminatorSymbol: IfcPreDefinedSymbol
    {
        public override string WhereRule()
        {
            var result = base.WhereRule();

            if (!new[] { "blanked arrow","blanked box", "blanked dot","dimension origin","filled arrow","filled box", "filled dot","integral symbol","open arrow","slash","unfilled arrow" }.Any(v => v == Name))
                result += "WR31: The inherited name for pre defined items shall only have the value of one of the following words: 'blanked arrow','blanked box', 'blanked dot','dimension origin','filled arrow','filled box', 'filled dot','integral symbol','open arrow','slash','unfilled arrow' \n";

            return result;
        }

    }
}
