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
namespace Xbim.Ifc2x3.GeometryResource
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
			if (clause == Where.IfcAxis1Placement.WR1) {
				try {
					retVal = (!(EXISTS(Axis))) || (Axis.Dim == 3);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcAxis1Placement");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAxis1Placement.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAxis1Placement.WR2) {
				try {
					retVal = this/* as IfcPlacement*/.Location.Dim == 3;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcAxis1Placement");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAxis1Placement.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcAxis1Placement.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis1Placement.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAxis1Placement.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis1Placement.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcAxis1Placement
	{
		public static readonly IfcAxis1Placement WR1 = new IfcAxis1Placement();
		public static readonly IfcAxis1Placement WR2 = new IfcAxis1Placement();
		protected IfcAxis1Placement() {}
	}
}
