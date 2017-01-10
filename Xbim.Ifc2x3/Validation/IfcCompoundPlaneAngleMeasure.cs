using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
using static Xbim.Ifc2x3.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.MeasureResource
{
	public partial struct IfcCompoundPlaneAngleMeasure : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCompoundPlaneAngleMeasure clause) {
			var retVal = false;
			if (clause == Where.IfcCompoundPlaneAngleMeasure.WR1) {
				try {
					retVal = ((-360 <= this.ItemAt(0)) && (this.ItemAt(0) < 360) );
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.MeasureResource.IfcCompoundPlaneAngleMeasure");
					Log.Error("Exception thrown evaluating where-clause 'IfcCompoundPlaneAngleMeasure.WR1'.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCompoundPlaneAngleMeasure.WR2) {
				try {
					retVal = ((-60 <= this.ItemAt(1)) && (this.ItemAt(1) < 60) );
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.MeasureResource.IfcCompoundPlaneAngleMeasure");
					Log.Error("Exception thrown evaluating where-clause 'IfcCompoundPlaneAngleMeasure.WR2'.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCompoundPlaneAngleMeasure.WR3) {
				try {
					retVal = ((-60 <= this.ItemAt(2)) && (this.ItemAt(2) < 60) );
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.MeasureResource.IfcCompoundPlaneAngleMeasure");
					Log.Error("Exception thrown evaluating where-clause 'IfcCompoundPlaneAngleMeasure.WR3'.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCompoundPlaneAngleMeasure.WR4) {
				try {
					retVal = ((this.ItemAt(0) >= 0) && (this.ItemAt(1) >= 0) && (this.ItemAt(2) >= 0)) || ((this.ItemAt(0) <= 0) && (this.ItemAt(1) <= 0) && (this.ItemAt(2) <= 0));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.MeasureResource.IfcCompoundPlaneAngleMeasure");
					Log.Error("Exception thrown evaluating where-clause 'IfcCompoundPlaneAngleMeasure.WR4'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public  IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcCompoundPlaneAngleMeasure.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompoundPlaneAngleMeasure.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCompoundPlaneAngleMeasure.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompoundPlaneAngleMeasure.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCompoundPlaneAngleMeasure.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompoundPlaneAngleMeasure.WR3", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCompoundPlaneAngleMeasure.WR4))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompoundPlaneAngleMeasure.WR4", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcCompoundPlaneAngleMeasure
	{
		public static readonly IfcCompoundPlaneAngleMeasure WR1 = new IfcCompoundPlaneAngleMeasure();
		public static readonly IfcCompoundPlaneAngleMeasure WR2 = new IfcCompoundPlaneAngleMeasure();
		public static readonly IfcCompoundPlaneAngleMeasure WR3 = new IfcCompoundPlaneAngleMeasure();
		public static readonly IfcCompoundPlaneAngleMeasure WR4 = new IfcCompoundPlaneAngleMeasure();
		protected IfcCompoundPlaneAngleMeasure() {}
	}
}
