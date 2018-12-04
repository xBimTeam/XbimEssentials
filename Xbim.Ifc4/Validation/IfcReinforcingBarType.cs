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
namespace Xbim.Ifc4.StructuralElementsDomain
{
	public partial class IfcReinforcingBarType : IExpressValidatable
	{
		public enum IfcReinforcingBarTypeClause
		{
			CorrectPredefinedType,
			BendingShapeCodeProvided,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcReinforcingBarTypeClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcReinforcingBarTypeClause.CorrectPredefinedType:
						retVal = (PredefinedType != IfcReinforcingBarTypeEnum.USERDEFINED) || ((PredefinedType == IfcReinforcingBarTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcElementType*/.ElementType));
						break;
					case IfcReinforcingBarTypeClause.BendingShapeCodeProvided:
						retVal = !Functions.EXISTS(BendingParameters) || Functions.EXISTS(BendingShapeCode);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.StructuralElementsDomain.IfcReinforcingBarType>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcReinforcingBarType.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcReinforcingBarTypeClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcReinforcingBarType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcReinforcingBarTypeClause.BendingShapeCodeProvided))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcReinforcingBarType.BendingShapeCodeProvided", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
