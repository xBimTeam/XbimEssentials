using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
using static Xbim.Ifc4.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.ProductExtension
{
	public partial class IfcRelInterferesElements : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcRelInterferesElements");

		/// <summary>
		/// Tests the express where clause NotSelfReference
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NotSelfReference() {
			var retVal = false;
			try {
				retVal = !Object.ReferenceEquals(RelatingElement, RelatedElement);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NotSelfReference' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!NotSelfReference())
				yield return new ValidationResult() { Item = this, IssueSource = "NotSelfReference", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
