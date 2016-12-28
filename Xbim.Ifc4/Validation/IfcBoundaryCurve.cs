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
namespace Xbim.Ifc4.GeometryResource
{
	public partial class IfcBoundaryCurve : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcBoundaryCurve");

		/// <summary>
		/// Tests the express where clause IsClosed
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool IsClosed() {
			var retVal = false;
			try {
				retVal = this/* as IfcCompositeCurve*/.ClosedCurve.AsBool();
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'IsClosed' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!IsClosed())
				yield return new ValidationResult() { Item = this, IssueSource = "IsClosed", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
