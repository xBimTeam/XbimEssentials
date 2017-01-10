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
namespace Xbim.Ifc4.SharedBldgElements
{
	public partial class IfcWallStandardCase : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcWallStandardCase clause) {
			var retVal = false;
			if (clause == Where.IfcWallStandardCase.HasMaterialLayerSetUsage) {
				try {
					retVal = SIZEOF(USEDIN(this, "IFC4.IFCRELASSOCIATES.RELATEDOBJECTS").Where(temp => (TYPEOF(temp).Contains("IFC4.IFCRELASSOCIATESMATERIAL")) && (TYPEOF(temp.AsIfcRelAssociatesMaterial().RelatingMaterial).Contains("IFC4.IFCMATERIALLAYERSETUSAGE")))) == 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedBldgElements.IfcWallStandardCase");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcWallStandardCase.HasMaterialLayerSetUsage' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcWall)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcWallStandardCase.HasMaterialLayerSetUsage))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcWallStandardCase.HasMaterialLayerSetUsage", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcWallStandardCase : IfcWall
	{
		public static readonly IfcWallStandardCase HasMaterialLayerSetUsage = new IfcWallStandardCase();
		protected IfcWallStandardCase() {}
	}
}
