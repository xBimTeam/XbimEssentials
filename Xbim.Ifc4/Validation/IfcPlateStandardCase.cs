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
	public partial class IfcPlateStandardCase : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedBldgElements.IfcPlateStandardCase");

		/// <summary>
		/// Tests the express where clause HasMaterialLayerSetUsage
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool HasMaterialLayerSetUsage() {
			var retVal = false;
			try {
				retVal = SIZEOF(USEDIN(this, "IFC4.IFCRELASSOCIATES.RELATEDOBJECTS").Where(temp => (TYPEOF(temp).Contains("IFC4.IFCRELASSOCIATESMATERIAL")) && (TYPEOF(temp.AsIfcRelAssociatesMaterial().RelatingMaterial).Contains("IFC4.IFCMATERIALLAYERSETUSAGE")))) == 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'HasMaterialLayerSetUsage' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!HasMaterialLayerSetUsage())
				yield return new ValidationResult() { Item = this, IssueSource = "HasMaterialLayerSetUsage", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
