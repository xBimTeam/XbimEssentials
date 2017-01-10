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
namespace Xbim.Ifc4.GeometricModelResource
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
			if (clause == Where.IfcPolygonalBoundedHalfSpace.BoundaryDim) {
				try {
					retVal = PolygonalBoundary.Dim == 2;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcPolygonalBoundedHalfSpace");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPolygonalBoundedHalfSpace.BoundaryDim' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPolygonalBoundedHalfSpace.BoundaryType) {
				try {
					retVal = SIZEOF(TYPEOF(PolygonalBoundary) * NewArray("IFC4.IFCPOLYLINE", "IFC4.IFCCOMPOSITECURVE")) == 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcPolygonalBoundedHalfSpace");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPolygonalBoundedHalfSpace.BoundaryType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPolygonalBoundedHalfSpace.BoundaryDim))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPolygonalBoundedHalfSpace.BoundaryDim", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPolygonalBoundedHalfSpace.BoundaryType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPolygonalBoundedHalfSpace.BoundaryType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPolygonalBoundedHalfSpace
	{
		public static readonly IfcPolygonalBoundedHalfSpace BoundaryDim = new IfcPolygonalBoundedHalfSpace();
		public static readonly IfcPolygonalBoundedHalfSpace BoundaryType = new IfcPolygonalBoundedHalfSpace();
		protected IfcPolygonalBoundedHalfSpace() {}
	}
}
