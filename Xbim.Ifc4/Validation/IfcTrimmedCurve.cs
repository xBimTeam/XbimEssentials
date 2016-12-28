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
	public partial class IfcTrimmedCurve : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcTrimmedCurve");

		/// <summary>
		/// Tests the express where clause Trim1ValuesConsistent
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool Trim1ValuesConsistent() {
			var retVal = false;
			try {
				retVal = (HIINDEX(Trim1) == 1) || (TYPEOF(Trim1.ToArray()[0]) != TYPEOF(Trim1.ToArray()[1]));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'Trim1ValuesConsistent' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause Trim2ValuesConsistent
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool Trim2ValuesConsistent() {
			var retVal = false;
			try {
				retVal = (HIINDEX(Trim2) == 1) || (TYPEOF(Trim2.ToArray()[0]) != TYPEOF(Trim2.ToArray()[1]));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'Trim2ValuesConsistent' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause NoTrimOfBoundedCurves
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NoTrimOfBoundedCurves() {
			var retVal = false;
			try {
				retVal = !(TYPEOF(BasisCurve).Contains("IFC4.IFCBOUNDEDCURVE"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NoTrimOfBoundedCurves' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!Trim1ValuesConsistent())
				yield return new ValidationResult() { Item = this, IssueSource = "Trim1ValuesConsistent", IssueType = ValidationFlags.EntityWhereClauses };
			if (!Trim2ValuesConsistent())
				yield return new ValidationResult() { Item = this, IssueSource = "Trim2ValuesConsistent", IssueType = ValidationFlags.EntityWhereClauses };
			if (!NoTrimOfBoundedCurves())
				yield return new ValidationResult() { Item = this, IssueSource = "NoTrimOfBoundedCurves", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
