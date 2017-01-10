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
namespace Xbim.Ifc2x3.GeometricConstraintResource
{
	public partial class IfcGridAxis : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcGridAxis clause) {
			var retVal = false;
			if (clause == Where.IfcGridAxis.WR1) {
				try {
					retVal = AxisCurve.Dim == 2;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometricConstraintResource.IfcGridAxis");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcGridAxis.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcGridAxis.WR2) {
				try {
					retVal = (SIZEOF(PartOfU) == 1) ^ (SIZEOF(PartOfV) == 1) ^ (SIZEOF(PartOfW) == 1);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometricConstraintResource.IfcGridAxis");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcGridAxis.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcGridAxis.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGridAxis.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcGridAxis.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGridAxis.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcGridAxis
	{
		public static readonly IfcGridAxis WR1 = new IfcGridAxis();
		public static readonly IfcGridAxis WR2 = new IfcGridAxis();
		protected IfcGridAxis() {}
	}
}
