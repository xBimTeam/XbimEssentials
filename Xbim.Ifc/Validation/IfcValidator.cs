using System;
using Xbim.Common.ExpressValidation;

namespace Xbim.Ifc.Validation
{
    /// <summary>
    /// [Deprecated] use Xbim.Common.ExpressValidation.Validator instead
    /// This class provides basic POCO access to validation errors.
    /// Validation reporting should build upon this. For an example see <see cref="IfcValidationReporter"/>
    /// </summary>
    [Obsolete("This class is not IFC specific so it was moved to Xbim.Common.ExpressValidation.Validator.")]
    public class IfcValidator: Validator
    {
       
    }
}
