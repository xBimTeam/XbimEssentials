using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.StructuralLoadResource
{
	public partial class IfcSurfaceReinforcementArea : IExpressValidatable
	{
		public enum IfcSurfaceReinforcementAreaClause
		{
			SurfaceAndOrShearAreaSpecified,
			NonnegativeArea1,
			NonnegativeArea2,
			NonnegativeArea3,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcSurfaceReinforcementAreaClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcSurfaceReinforcementAreaClause.SurfaceAndOrShearAreaSpecified:
						retVal = Functions.EXISTS(SurfaceReinforcement1) || Functions.EXISTS(SurfaceReinforcement2) || Functions.EXISTS(ShearReinforcement);
						break;
					case IfcSurfaceReinforcementAreaClause.NonnegativeArea1:
						retVal = (!Functions.EXISTS(SurfaceReinforcement1)) || ((SurfaceReinforcement1.ItemAt(0) >= 0) && (SurfaceReinforcement1.ItemAt(1) >= 0) && ((Functions.SIZEOF(SurfaceReinforcement1) == 1) || (SurfaceReinforcement1.ItemAt(0) >= 0)));
						break;
					case IfcSurfaceReinforcementAreaClause.NonnegativeArea2:
						retVal = (!Functions.EXISTS(SurfaceReinforcement2)) || ((SurfaceReinforcement2.ItemAt(0) >= 0) && (SurfaceReinforcement2.ItemAt(1) >= 0) && ((Functions.SIZEOF(SurfaceReinforcement2) == 1) || (SurfaceReinforcement2.ItemAt(0) >= 0)));
						break;
					case IfcSurfaceReinforcementAreaClause.NonnegativeArea3:
						retVal = (!Functions.EXISTS(ShearReinforcement)) || (ShearReinforcement >= 0);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.StructuralLoadResource.IfcSurfaceReinforcementArea>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcSurfaceReinforcementArea.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcSurfaceReinforcementAreaClause.SurfaceAndOrShearAreaSpecified))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceReinforcementArea.SurfaceAndOrShearAreaSpecified", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSurfaceReinforcementAreaClause.NonnegativeArea1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceReinforcementArea.NonnegativeArea1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSurfaceReinforcementAreaClause.NonnegativeArea2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceReinforcementArea.NonnegativeArea2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSurfaceReinforcementAreaClause.NonnegativeArea3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceReinforcementArea.NonnegativeArea3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
