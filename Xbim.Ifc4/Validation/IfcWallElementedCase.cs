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
	public partial class IfcWallElementedCase : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedBldgElements.IfcWallElementedCase");

		/// <summary>
		/// Tests the express where clause HasDecomposition
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool HasDecomposition() {
			var retVal = false;
			try {
				retVal = HIINDEX(this/* as IfcObjectDefinition*/.IsDecomposedBy) > 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'HasDecomposition' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!HasDecomposition())
				yield return new ValidationResult() { Item = this, IssueSource = "HasDecomposition", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
