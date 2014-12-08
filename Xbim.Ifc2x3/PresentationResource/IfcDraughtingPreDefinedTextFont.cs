using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;

namespace Xbim.Ifc2x3.PresentationResource
{
    /// <summary>
    /// The draughting pre defined text font is a pre defined text font for the purpose to identify a font by name. 
    /// Allowable names are:
    /// 
    ///     'ISO 3098-1 font A',
    ///     'ISO 3098-1 font B',
    /// 
    /// The ISO 3098-1 font A is the text font as denoted as Letterng A in clause 3 of ISO 3098-1, the ISO 3098-1 font 
    /// B is the text font as denoted as Letterng B in clause 3 of ISO 3098-1.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcDraughtingPreDefinedTextFont : IfcPreDefinedTextFont
    {
        public override string WhereRule()
        {
            var result = "";
            if (!new[] { "ISO 3098-1 font A", "ISO 3098-1 font B" }.Any(v => v == Name))
                result += "WR31: The inherited name for pre defined items shall only have the value of one of the following words: ['ISO 3098-1 font A','ISO 3098-1 font B'] \n";
            
            return result;
        }
    }
}
