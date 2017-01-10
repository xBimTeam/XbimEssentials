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
	public partial class IfcWasteTerminal : IExpressValidatable
	{
		public enum IfcWasteTerminalClause
		{
			CorrectPredefinedType,
			CorrectTypeAssigned,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcWasteTerminalClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcWasteTerminalClause.CorrectPredefinedType:
						retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcWasteTerminalTypeEnum.USERDEFINED) || ((PredefinedType == IfcWasteTerminalTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
						break;
					case IfcWasteTerminalClause.CorrectTypeAssigned:
						retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCWASTETERMINALTYPE"));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.PlumbingFireProtectionDomain.IfcWasteTerminal");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcWasteTerminal.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcWasteTerminalClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcWasteTerminal.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcWasteTerminalClause.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcWasteTerminal.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
