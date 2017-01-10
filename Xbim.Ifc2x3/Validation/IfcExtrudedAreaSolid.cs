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
	public partial class IfcExtrudedAreaSolid : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcExtrudedAreaSolid clause) {
			var retVal = false;
			if (clause == Where.IfcExtrudedAreaSolid.WR31) {
				try {
					retVal = IfcDotProduct(IfcDirection(0, 0, 1), this.ExtrudedDirection) != 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometricModelResource.IfcExtrudedAreaSolid");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcExtrudedAreaSolid.WR31' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcSweptAreaSolid)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcExtrudedAreaSolid.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcExtrudedAreaSolid.WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcExtrudedAreaSolid : IfcSweptAreaSolid
	{
		public static readonly IfcExtrudedAreaSolid WR31 = new IfcExtrudedAreaSolid();
		protected IfcExtrudedAreaSolid() {}
	}
}
