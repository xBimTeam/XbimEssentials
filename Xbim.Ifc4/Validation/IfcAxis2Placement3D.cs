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
	public partial class IfcAxis2Placement3D : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcAxis2Placement3D clause) {
			var retVal = false;
			if (clause == Where.IfcAxis2Placement3D.LocationIs3D) {
				try {
					retVal = this/* as IfcPlacement*/.Location.Dim == 3;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcAxis2Placement3D");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAxis2Placement3D.LocationIs3D' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAxis2Placement3D.AxisIs3D) {
				try {
					retVal = (!(EXISTS(Axis))) || (Axis.Dim == 3);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcAxis2Placement3D");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAxis2Placement3D.AxisIs3D' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAxis2Placement3D.RefDirIs3D) {
				try {
					retVal = (!(EXISTS(RefDirection))) || (RefDirection.Dim == 3);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcAxis2Placement3D");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAxis2Placement3D.RefDirIs3D' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAxis2Placement3D.AxisToRefDirPosition) {
				try {
					retVal = (!(EXISTS(Axis))) || (!(EXISTS(RefDirection))) || (IfcCrossProduct(Axis, RefDirection).Magnitude > 0);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcAxis2Placement3D");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAxis2Placement3D.AxisToRefDirPosition' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAxis2Placement3D.AxisAndRefDirProvision) {
				try {
					retVal = !((EXISTS(Axis)) ^ (EXISTS(RefDirection)));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcAxis2Placement3D");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAxis2Placement3D.AxisAndRefDirProvision' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcAxis2Placement3D.LocationIs3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.LocationIs3D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAxis2Placement3D.AxisIs3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.AxisIs3D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAxis2Placement3D.RefDirIs3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.RefDirIs3D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAxis2Placement3D.AxisToRefDirPosition))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.AxisToRefDirPosition", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAxis2Placement3D.AxisAndRefDirProvision))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.AxisAndRefDirProvision", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcAxis2Placement3D
	{
		public static readonly IfcAxis2Placement3D LocationIs3D = new IfcAxis2Placement3D();
		public static readonly IfcAxis2Placement3D AxisIs3D = new IfcAxis2Placement3D();
		public static readonly IfcAxis2Placement3D RefDirIs3D = new IfcAxis2Placement3D();
		public static readonly IfcAxis2Placement3D AxisToRefDirPosition = new IfcAxis2Placement3D();
		public static readonly IfcAxis2Placement3D AxisAndRefDirProvision = new IfcAxis2Placement3D();
		protected IfcAxis2Placement3D() {}
	}
}
