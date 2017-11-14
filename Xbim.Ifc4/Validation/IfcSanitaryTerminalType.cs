using System;
using log4net;
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
	public partial class IfcSanitaryTerminalType : IExpressValidatable
	{
		public enum IfcSanitaryTerminalTypeClause
		{
			CorrectPredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcSanitaryTerminalTypeClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcSanitaryTerminalTypeClause.CorrectPredefinedType:
						retVal = (PredefinedType != IfcSanitaryTerminalTypeEnum.USERDEFINED) || ((PredefinedType == IfcSanitaryTerminalTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcElementType*/.ElementType));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.PlumbingFireProtectionDomain.IfcSanitaryTerminalType");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSanitaryTerminalType.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcSanitaryTerminalTypeClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSanitaryTerminalType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
