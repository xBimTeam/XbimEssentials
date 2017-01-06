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
	public partial class IfcSlabStandardCase : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedBldgElements.IfcSlabStandardCase");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSlabStandardCase clause) {
			var retVal = false;
			if (clause == Where.IfcSlabStandardCase.HasMaterialLayerSetusage) {
				try {
					retVal = SIZEOF(USEDIN(this, "IFC4.IFCRELASSOCIATES.RELATEDOBJECTS").Where(temp => (TYPEOF(temp).Contains("IFC4.IFCRELASSOCIATESMATERIAL")) && (TYPEOF(temp.AsIfcRelAssociatesMaterial().RelatingMaterial).Contains("IFC4.IFCMATERIALLAYERSETUSAGE")))) == 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSlabStandardCase.HasMaterialLayerSetusage' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcSlab)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcSlabStandardCase.HasMaterialLayerSetusage))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSlabStandardCase.HasMaterialLayerSetusage", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcSlabStandardCase : IfcSlab
	{
		public static readonly IfcSlabStandardCase HasMaterialLayerSetusage = new IfcSlabStandardCase();
		protected IfcSlabStandardCase() {}
	}
}
