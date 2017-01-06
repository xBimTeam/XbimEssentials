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
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTrimmedCurve clause) {
			var retVal = false;
			if (clause == Where.IfcTrimmedCurve.Trim1ValuesConsistent) {
				try {
					retVal = (HIINDEX(Trim1) == 1) || (TYPEOF(Trim1.ToArray()[0]) != TYPEOF(Trim1.ToArray()[1]));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcTrimmedCurve.Trim1ValuesConsistent' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcTrimmedCurve.Trim2ValuesConsistent) {
				try {
					retVal = (HIINDEX(Trim2) == 1) || (TYPEOF(Trim2.ToArray()[0]) != TYPEOF(Trim2.ToArray()[1]));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcTrimmedCurve.Trim2ValuesConsistent' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcTrimmedCurve.NoTrimOfBoundedCurves) {
				try {
					retVal = !(TYPEOF(BasisCurve).Contains("IFC4.IFCBOUNDEDCURVE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcTrimmedCurve.NoTrimOfBoundedCurves' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcTrimmedCurve.Trim1ValuesConsistent))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTrimmedCurve.Trim1ValuesConsistent", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcTrimmedCurve.Trim2ValuesConsistent))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTrimmedCurve.Trim2ValuesConsistent", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcTrimmedCurve.NoTrimOfBoundedCurves))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTrimmedCurve.NoTrimOfBoundedCurves", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcTrimmedCurve
	{
		public static readonly IfcTrimmedCurve Trim1ValuesConsistent = new IfcTrimmedCurve();
		public static readonly IfcTrimmedCurve Trim2ValuesConsistent = new IfcTrimmedCurve();
		public static readonly IfcTrimmedCurve NoTrimOfBoundedCurves = new IfcTrimmedCurve();
		protected IfcTrimmedCurve() {}
	}
}
