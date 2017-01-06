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
namespace Xbim.Ifc4.MeasureResource
{
	public partial struct IfcCompoundPlaneAngleMeasure : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.MeasureResource.IfcCompoundPlaneAngleMeasure");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCompoundPlaneAngleMeasure clause) {
			var retVal = false;
			if (clause == Where.IfcCompoundPlaneAngleMeasure.MinutesInRange) {
				try {
					retVal = ABS(this.ToArray()[1]) < 60;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCompoundPlaneAngleMeasure.MinutesInRange'.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCompoundPlaneAngleMeasure.SecondsInRange) {
				try {
					retVal = ABS(this.ToArray()[2]) < 60;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCompoundPlaneAngleMeasure.SecondsInRange'.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCompoundPlaneAngleMeasure.MicrosecondsInRange) {
				try {
					retVal = (SIZEOF(this) == 3) || (ABS(this.ToArray()[3]) < 1000000);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCompoundPlaneAngleMeasure.MicrosecondsInRange'.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCompoundPlaneAngleMeasure.ConsistentSign) {
				try {
					retVal = ((this.ToArray()[0] >= 0) && (this.ToArray()[1] >= 0) && (this.ToArray()[2] >= 0) && ((SIZEOF(this) == 3) || (this.ToArray()[3] >= 0))) || ((this.ToArray()[0] <= 0) && (this.ToArray()[1] <= 0) && (this.ToArray()[2] <= 0) && ((SIZEOF(this) == 3) || (this.ToArray()[3] <= 0)));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCompoundPlaneAngleMeasure.ConsistentSign'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcCompoundPlaneAngleMeasure.MinutesInRange))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompoundPlaneAngleMeasure.MinutesInRange", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCompoundPlaneAngleMeasure.SecondsInRange))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompoundPlaneAngleMeasure.SecondsInRange", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCompoundPlaneAngleMeasure.MicrosecondsInRange))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompoundPlaneAngleMeasure.MicrosecondsInRange", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCompoundPlaneAngleMeasure.ConsistentSign))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompoundPlaneAngleMeasure.ConsistentSign", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcCompoundPlaneAngleMeasure
	{
		public static readonly IfcCompoundPlaneAngleMeasure MinutesInRange = new IfcCompoundPlaneAngleMeasure();
		public static readonly IfcCompoundPlaneAngleMeasure SecondsInRange = new IfcCompoundPlaneAngleMeasure();
		public static readonly IfcCompoundPlaneAngleMeasure MicrosecondsInRange = new IfcCompoundPlaneAngleMeasure();
		public static readonly IfcCompoundPlaneAngleMeasure ConsistentSign = new IfcCompoundPlaneAngleMeasure();
		protected IfcCompoundPlaneAngleMeasure() {}
	}
}
