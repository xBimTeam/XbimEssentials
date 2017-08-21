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
namespace Xbim.Ifc4.HvacDomain
{
	public partial class IfcAirToAirHeatRecovery : IExpressValidatable
	{
		public enum IfcAirToAirHeatRecoveryClause
		{
			CorrectPredefinedType,
			CorrectTypeAssigned,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcAirToAirHeatRecoveryClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcAirToAirHeatRecoveryClause.CorrectPredefinedType:
						retVal = !(Functions.EXISTS(PredefinedType)) || (PredefinedType != IfcAirToAirHeatRecoveryTypeEnum.USERDEFINED) || ((PredefinedType == IfcAirToAirHeatRecoveryTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcObject*/.ObjectType));
						break;
					case IfcAirToAirHeatRecoveryClause.CorrectTypeAssigned:
						retVal = (Functions.SIZEOF(IsTypedBy) == 0) || (Functions.TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCAIRTOAIRHEATRECOVERYTYPE"));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.HvacDomain.IfcAirToAirHeatRecovery>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcAirToAirHeatRecovery.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcAirToAirHeatRecoveryClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAirToAirHeatRecovery.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAirToAirHeatRecoveryClause.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAirToAirHeatRecovery.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
