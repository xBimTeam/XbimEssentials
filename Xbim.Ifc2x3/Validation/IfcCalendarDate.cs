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
namespace Xbim.Ifc2x3.DateTimeResource
{
	public partial class IfcCalendarDate : IExpressValidatable
	{
		public enum IfcCalendarDateClause
		{
			WR21,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCalendarDateClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCalendarDateClause.WR21:
						retVal = IfcValidCalendarDate(this);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc2x3.DateTimeResource.IfcCalendarDate");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCalendarDate.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcCalendarDateClause.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCalendarDate.WR21", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
