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
	public partial class IfcEventType : IExpressValidatable
	{
		public enum IfcEventTypeClause
		{
			CorrectPredefinedType,
			CorrectEventTriggerType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcEventTypeClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcEventTypeClause.CorrectPredefinedType:
						retVal = (PredefinedType != IfcEventTypeEnum.USERDEFINED) || ((PredefinedType == IfcEventTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcTypeProcess*/.ProcessType));
						break;
					case IfcEventTypeClause.CorrectEventTriggerType:
						retVal = (EventTriggerType != IfcEventTriggerTypeEnum.USERDEFINED) || ((EventTriggerType == IfcEventTriggerTypeEnum.USERDEFINED) && Functions.EXISTS(UserDefinedEventTriggerType));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProcessExtension.IfcEventType>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcEventType.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcEventTypeClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEventType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcEventTypeClause.CorrectEventTriggerType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEventType.CorrectEventTriggerType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
