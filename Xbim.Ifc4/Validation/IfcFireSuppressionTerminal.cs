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
namespace Xbim.Ifc4.PlumbingFireProtectionDomain
{
	public partial class IfcFireSuppressionTerminal : IExpressValidatable
	{
		public enum IfcFireSuppressionTerminalClause
		{
			CorrectPredefinedType,
			CorrectTypeAssigned,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcFireSuppressionTerminalClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcFireSuppressionTerminalClause.CorrectPredefinedType:
						retVal = !(Functions.EXISTS(PredefinedType)) || (PredefinedType != IfcFireSuppressionTerminalTypeEnum.USERDEFINED) || ((PredefinedType == IfcFireSuppressionTerminalTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcObject*/.ObjectType));
						break;
					case IfcFireSuppressionTerminalClause.CorrectTypeAssigned:
						retVal = (Functions.SIZEOF(IsTypedBy) == 0) || (Functions.TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCFIRESUPPRESSIONTERMINALTYPE"));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.PlumbingFireProtectionDomain.IfcFireSuppressionTerminal>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcFireSuppressionTerminal.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcFireSuppressionTerminalClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFireSuppressionTerminal.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcFireSuppressionTerminalClause.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFireSuppressionTerminal.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
