using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
using static Xbim.Ifc2x3.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.ControlExtension
{
	public partial class IfcTimeSeriesSchedule : IExpressValidatable
	{
		public enum IfcTimeSeriesScheduleClause
		{
			WR41,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcTimeSeriesScheduleClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTimeSeriesScheduleClause.WR41:
						retVal = !(TimeSeriesScheduleType == IfcTimeSeriesScheduleTypeEnum.USERDEFINED) || EXISTS(this/* as IfcObject*/.ObjectType);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc2x3.ControlExtension.IfcTimeSeriesSchedule");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTimeSeriesSchedule.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcTimeSeriesScheduleClause.WR41))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTimeSeriesSchedule.WR41", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
