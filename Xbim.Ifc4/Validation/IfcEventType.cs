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
	public partial class IfcEventType : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProcessExtension.IfcEventType");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcEventType clause) {
			var retVal = false;
			if (clause == Where.IfcEventType.CorrectPredefinedType) {
				try {
					retVal = (PredefinedType != IfcEventTypeEnum.USERDEFINED) || ((PredefinedType == IfcEventTypeEnum.USERDEFINED) && EXISTS(this/* as IfcTypeProcess*/.ProcessType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcEventType.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcEventType.CorrectEventTriggerType) {
				try {
					retVal = (EventTriggerType != IfcEventTriggerTypeEnum.USERDEFINED) || ((EventTriggerType == IfcEventTriggerTypeEnum.USERDEFINED) && EXISTS(UserDefinedEventTriggerType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcEventType.CorrectEventTriggerType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcTypeObject)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcEventType.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEventType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcEventType.CorrectEventTriggerType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEventType.CorrectEventTriggerType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcEventType : IfcTypeObject
	{
		public static readonly IfcEventType CorrectPredefinedType = new IfcEventType();
		public static readonly IfcEventType CorrectEventTriggerType = new IfcEventType();
		protected IfcEventType() {}
	}
}
