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
namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
	public partial class IfcStructuralSurfaceMemberVarying : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStructuralSurfaceMemberVarying clause) {
			var retVal = false;
			if (clause == Where.IfcStructuralSurfaceMemberVarying.WR61) {
				try {
					retVal = EXISTS(this/* as IfcStructuralSurfaceMember*/.Thickness);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.StructuralAnalysisDomain.IfcStructuralSurfaceMemberVarying");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralSurfaceMemberVarying.WR61' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcStructuralSurfaceMemberVarying.WR62) {
				try {
					retVal = SIZEOF(this.VaryingThicknessLocation.ShapeRepresentations.Where(temp => !(SIZEOF(temp/* as IfcRepresentation*/.Items) == 1))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.StructuralAnalysisDomain.IfcStructuralSurfaceMemberVarying");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralSurfaceMemberVarying.WR62' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcStructuralSurfaceMemberVarying.WR63) {
				try {
					retVal = SIZEOF(this.VaryingThicknessLocation.ShapeRepresentations.Where(temp => !((TYPEOF(temp/* as IfcRepresentation*/.Items.ItemAt(0)).Contains("IFC2X3.IFCCARTESIANPOINT")) || (TYPEOF(temp/* as IfcRepresentation*/.Items.ItemAt(0)).Contains("IFC2X3.IFCPOINTONSURFACE"))))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.StructuralAnalysisDomain.IfcStructuralSurfaceMemberVarying");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralSurfaceMemberVarying.WR63' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcProduct)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcStructuralSurfaceMemberVarying.WR61))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralSurfaceMemberVarying.WR61", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcStructuralSurfaceMemberVarying.WR62))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralSurfaceMemberVarying.WR62", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcStructuralSurfaceMemberVarying.WR63))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralSurfaceMemberVarying.WR63", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcStructuralSurfaceMemberVarying : IfcProduct
	{
		public static readonly IfcStructuralSurfaceMemberVarying WR61 = new IfcStructuralSurfaceMemberVarying();
		public static readonly IfcStructuralSurfaceMemberVarying WR62 = new IfcStructuralSurfaceMemberVarying();
		public static readonly IfcStructuralSurfaceMemberVarying WR63 = new IfcStructuralSurfaceMemberVarying();
		protected IfcStructuralSurfaceMemberVarying() {}
	}
}
