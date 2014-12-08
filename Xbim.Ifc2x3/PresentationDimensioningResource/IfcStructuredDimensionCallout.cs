using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.PresentationDefinitionResource;
using Xbim.XbimExtensions;

namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
    [IfcPersistedEntity]
    public class IfcStructuredDimensionCallout : IfcDraughtingCallout
    {
        public override string WhereRule()
        {
            var result = base.WhereRule();

            var names = new[] { "dimension value", "tolerance value","unit text","prefix text","suffix text"};
            foreach (var txt in Contents.OfType<IfcAnnotationTextOccurrence>())
            {
                if (!names.Any(n => n == txt.Name))
                {
                    result += "WR31: Each annotation text occurrence within the set of contents referenced by the structured dimension callout shall have a name of either of the following: ‘dimension value’, ‘tolerance value’, ‘unit text’, ‘prefix text’, or ‘suffix text’. \n";
                    break;
                }
            } 

            return result;
        }
    }
}
