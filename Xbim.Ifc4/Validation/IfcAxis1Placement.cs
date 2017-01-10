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
	public partial class IfcAxis1Placement : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcAxis1Placement clause) {
			var retVal = false;
			if (clause == Where.IfcAxis1Placement.AxisIs3D) {
				try {
					retVal = (!(EXISTS(Axis))) || (Axis.Dim == 3);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcAxis1Placement");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAxis1Placement.AxisIs3D' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAxis1Placement.LocationIs3D) {
				try {
					retVal = this/* as IfcPlacement*/.Location.Dim == 3;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcAxis1Placement");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAxis1Placement.LocationIs3D' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcAxis1Placement.AxisIs3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis1Placement.AxisIs3D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAxis1Placement.LocationIs3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis1Placement.LocationIs3D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcAxis1Placement
	{
		public static readonly IfcAxis1Placement AxisIs3D = new IfcAxis1Placement();
		public static readonly IfcAxis1Placement LocationIs3D = new IfcAxis1Placement();
		protected IfcAxis1Placement() {}
	}
}
