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
namespace Xbim.Ifc4.ProcessExtension
{
	public partial class IfcEvent : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProcessExtension.IfcEvent");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcEvent clause) {
			var retVal = false;
			if (clause == Where.IfcEvent.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcEventTypeEnum.USERDEFINED) || ((PredefinedType == IfcEventTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcEvent.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcEvent.CorrectTypeAssigned) {
				try {
					retVal = !(EXISTS(EventTriggerType)) || (EventTriggerType != IfcEventTriggerTypeEnum.USERDEFINED) || ((EventTriggerType == IfcEventTriggerTypeEnum.USERDEFINED) && EXISTS(UserDefinedEventTriggerType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcEvent.CorrectTypeAssigned' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcObject)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcEvent.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEvent.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcEvent.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEvent.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcEvent : IfcObject
	{
		public static readonly IfcEvent CorrectPredefinedType = new IfcEvent();
		public static readonly IfcEvent CorrectTypeAssigned = new IfcEvent();
		protected IfcEvent() {}
	}
}
