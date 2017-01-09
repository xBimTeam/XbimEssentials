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
        private List<ValidationResult> _details;
        public ValidationResult Context;

        public void AddDetail(ValidationResult detail)
        {
            detail.Context = this;
            EnsureDetails();
            IssueType |= detail.IssueType; // adds the values of the detail
            _details.Add(detail);
        }

        public IEnumerable<ValidationResult> Details
        {
            get
            {
                EnsureDetails();
                return _details;
            }
        }

        private void EnsureDetails()
        {
            if (_details == null)
                _details = new List<ValidationResult>();
        }

        public string Report()
        {
            var msg = string.Format("Issue of type {0} on {1}.", IssueType, IssueSource);
            return msg;
        }
    }
}
