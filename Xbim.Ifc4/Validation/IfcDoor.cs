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
namespace Xbim.Ifc4.SharedBldgElements
{
	public partial class IfcDoor : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedBldgElements.IfcDoor");

		/// <summary>
		/// Tests the express where clause CorrectStyleAssigned
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrectStyleAssigned() {
			var retVal = false;
			try {
				retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCDOORTYPE"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrectStyleAssigned' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!CorrectStyleAssigned())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrectStyleAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
