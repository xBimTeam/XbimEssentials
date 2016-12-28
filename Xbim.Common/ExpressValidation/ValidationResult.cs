using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Common.Enumerations;

namespace Xbim.Common.ExpressValidation
{
    public class ValidationResult
    {
        public IPersist Item;
        public ValidationFlags IssueType;
        public string IssueSource;
        public string Message;
    }
}
