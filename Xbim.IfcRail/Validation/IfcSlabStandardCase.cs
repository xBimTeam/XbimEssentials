using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;

// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.IfcRail.SharedBldgElements
{
	public partial class IfcSlabStandardCase : IExpressValidatable
	{
		public enum IfcSlabStandardCaseClause
		{
			HasMaterialLayerSetusage,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcSlabStandardCaseClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcSlabStandardCaseClause.HasMaterialLayerSetusage:
						retVal = Functions.SIZEOF(Functions.USEDIN(this, "IFCRELASSOCIATES.RELATEDOBJECTS").Where(temp => (Functions.TYPEOF(temp).Contains("IFCRELASSOCIATESMATERIAL")) && (Functions.TYPEOF(temp.AsIfcRelAssociatesMaterial().RelatingMaterial).Contains("IFCMATERIALLAYERSETUSAGE")))) == 1;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.IfcRail.SharedBldgElements.IfcSlabStandardCase>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcSlabStandardCase.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcSlabStandardCaseClause.HasMaterialLayerSetusage))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSlabStandardCase.HasMaterialLayerSetusage", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
