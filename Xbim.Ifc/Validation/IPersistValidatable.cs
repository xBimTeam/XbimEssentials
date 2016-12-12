using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Ifc.Validation
{
    public interface IPersistValidatable
    {
        /// <summary>
        /// Validates the object against the Ifc schema where rule
        /// </summary>
        /// <returns>returns empty string if the object complies or an error string indicating the reason for compliance failure</returns>
        string WhereRule();
    }
}
