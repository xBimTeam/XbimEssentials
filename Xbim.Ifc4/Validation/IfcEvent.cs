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
namespace Xbim.Ifc4.ProcessExtension
{
	public partial class IfcEvent : IExpressValidatable
	{
		public enum IfcEventClause
		{
			CorrectPredefinedType,
			CorrectTypeAssigned,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcEventClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcEventClause.CorrectPredefinedType:
						retVal = !(Functions.EXISTS(PredefinedType)) || (PredefinedType != IfcEventTypeEnum.USERDEFINED) || ((PredefinedType == IfcEventTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcObject*/.ObjectType));
						break;
					case IfcEventClause.CorrectTypeAssigned:
						retVal = !(Functions.EXISTS(EventTriggerType)) || (EventTriggerType != IfcEventTriggerTypeEnum.USERDEFINED) || ((EventTriggerType == IfcEventTriggerTypeEnum.USERDEFINED) && Functions.EXISTS(UserDefinedEventTriggerType));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProcessExtension.IfcEvent>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcEvent.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcEventClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEvent.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcEventClause.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEvent.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
