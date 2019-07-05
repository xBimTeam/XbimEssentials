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
namespace Xbim.Ifc4.StructuralAnalysisDomain
{
	public partial class IfcStructuralPlanarAction : IExpressValidatable
	{
		public enum IfcStructuralPlanarActionClause
		{
			SuitableLoadType,
			ConstPredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcStructuralPlanarActionClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcStructuralPlanarActionClause.SuitableLoadType:
						retVal = Functions.SIZEOF(Functions.NewTypesArray("IFC4.IFCSTRUCTURALLOADPLANARFORCE", "IFC4.IFCSTRUCTURALLOADTEMPERATURE") * Functions.TYPEOF(this/* as IfcStructuralActivity*/.AppliedLoad)) == 1;
						break;
					case IfcStructuralPlanarActionClause.ConstPredefinedType:
						retVal = this/* as IfcStructuralSurfaceAction*/.PredefinedType == IfcStructuralSurfaceActivityTypeEnum.CONST;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralPlanarAction>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcStructuralPlanarAction.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcStructuralPlanarActionClause.SuitableLoadType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralPlanarAction.SuitableLoadType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcStructuralPlanarActionClause.ConstPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralPlanarAction.ConstPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
