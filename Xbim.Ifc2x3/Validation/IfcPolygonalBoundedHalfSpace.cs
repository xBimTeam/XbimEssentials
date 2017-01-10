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
namespace Xbim.Ifc2x3.GeometricModelResource
{
	public partial class IfcPolygonalBoundedHalfSpace : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPolygonalBoundedHalfSpace clause) {
			var retVal = false;
			if (clause == Where.IfcPolygonalBoundedHalfSpace.WR41) {
				try {
					retVal = PolygonalBoundary.Dim == 2;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometricModelResource.IfcPolygonalBoundedHalfSpace");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPolygonalBoundedHalfSpace.WR41' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPolygonalBoundedHalfSpace.WR42) {
				try {
					retVal = SIZEOF(TYPEOF(PolygonalBoundary) * NewArray("IFC2X3.IFCPOLYLINE", "IFC2X3.IFCCOMPOSITECURVE")) == 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometricModelResource.IfcPolygonalBoundedHalfSpace");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPolygonalBoundedHalfSpace.WR42' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPolygonalBoundedHalfSpace.WR41))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPolygonalBoundedHalfSpace.WR41", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPolygonalBoundedHalfSpace.WR42))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPolygonalBoundedHalfSpace.WR42", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcPolygonalBoundedHalfSpace
	{
		public static readonly IfcPolygonalBoundedHalfSpace WR41 = new IfcPolygonalBoundedHalfSpace();
		public static readonly IfcPolygonalBoundedHalfSpace WR42 = new IfcPolygonalBoundedHalfSpace();
		protected IfcPolygonalBoundedHalfSpace() {}
	}
}
