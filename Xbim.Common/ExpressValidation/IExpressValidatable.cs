using System.Collections.Generic;

namespace Xbim.Common.ExpressValidation
{
    public interface IExpressValidatable
    {
        IEnumerable<ValidationResult> Validate();
    }
}
