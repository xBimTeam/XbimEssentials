using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.PresentationDefinitionResource;
using Xbim.XbimExtensions;

namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
    /// <summary>
    /// The pre defined dimension symbol is a pre defined symbol for the purpose to identify a dimension symbol by name. Allowable names are:
    /// 
    /// 'arc length',
    /// 'conical taper',
    /// 'counterbore',
    /// 'countersink',
    /// 'depth',
    /// 'diameter',
    /// 'plus minus',
    /// 'slope',
    /// 'spherical diameter',
    /// 'spherical radius',
    /// 'square'
    /// 
    /// NOTE: The IfcPreDefinedDimensionSymbol is an entity that had been adopted from ISO 10303, Industrial automation systems and integration—Product data representation and exchange, Part 202: Application protocol: Associative draughting.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcPreDefinedDimensionSymbol : IfcPreDefinedSymbol
    {
        public override string WhereRule()
        {
            var result =  base.WhereRule();

            if (!new[] { "arc length","conical taper","counterbore", "countersink","depth","diameter","plus minus","radius", "slope","spherical diameter","spherical radius","square" }.Any(v => v == Name))
                result += "Allowable names are: 'arc length', 'conical taper', 'counterbore', 'countersink', 'depth', 'diameter', 'plus minus', 'slope', 'spherical diameter', 'spherical radius', 'square' \n";

            return result;
        }
    }
}
