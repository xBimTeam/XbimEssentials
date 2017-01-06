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
namespace Xbim.Ifc4.PlumbingFireProtectionDomain
{
	public partial class IfcFireSuppressionTerminal : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PlumbingFireProtectionDomain.IfcFireSuppressionTerminal");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcFireSuppressionTerminal clause) {
			var retVal = false;
			if (clause == Where.IfcFireSuppressionTerminal.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcFireSuppressionTerminalTypeEnum.USERDEFINED) || ((PredefinedType == IfcFireSuppressionTerminalTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcFireSuppressionTerminal.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcFireSuppressionTerminal.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCFIRESUPPRESSIONTERMINALTYPE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcFireSuppressionTerminal.CorrectTypeAssigned' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcProduct)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcFireSuppressionTerminal.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFireSuppressionTerminal.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcFireSuppressionTerminal.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFireSuppressionTerminal.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcFireSuppressionTerminal : IfcProduct
	{
		public static readonly IfcFireSuppressionTerminal CorrectPredefinedType = new IfcFireSuppressionTerminal();
		public static readonly IfcFireSuppressionTerminal CorrectTypeAssigned = new IfcFireSuppressionTerminal();
		protected IfcFireSuppressionTerminal() {}
	}
}
