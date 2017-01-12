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
namespace Xbim.Ifc4.ProcessExtension
{
	public partial class IfcWorkSchedule : IExpressValidatable
	{
		public enum IfcWorkScheduleClause
		{
			CorrectPredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcWorkScheduleClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcWorkScheduleClause.CorrectPredefinedType:
						retVal = !(Functions.EXISTS(PredefinedType)) || (PredefinedType != IfcWorkScheduleTypeEnum.USERDEFINED) || ((PredefinedType == IfcWorkScheduleTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcObject*/.ObjectType));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.ProcessExtension.IfcWorkSchedule");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcWorkSchedule.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcWorkScheduleClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcWorkSchedule.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
