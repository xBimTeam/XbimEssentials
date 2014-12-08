using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.PresentationDefinitionResource;
using Xbim.XbimExtensions;

namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
    [IfcPersistedEntity]
    public class IfcPreDefinedPointMarkerSymbol : IfcPreDefinedSymbol
    {
        public override string WhereRule()
        {
            var result = base.WhereRule();

            if (!new []{"asterisk","circle","dot","plus","square","triangle","x"}.Any(v => v == Name))
                result += "WR31: The inherited name for pre defined items shall only have the value of one of the following words: 'asterisk','circle','dot','plus','square','triangle','x'\n";

            return result;
        }
    }
}
