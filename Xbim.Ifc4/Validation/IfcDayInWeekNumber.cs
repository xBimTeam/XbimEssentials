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
namespace Xbim.Ifc4.DateTimeResource
{
	public partial struct IfcDayInWeekNumber : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.DateTimeResource.IfcDayInWeekNumber");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcDayInWeekNumber clause) {
			var retVal = false;
			if (clause == Where.IfcDayInWeekNumber.ValidRange) {
				try {
					retVal = ((1 <= this) && (this <= 7) );
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcDayInWeekNumber.ValidRange'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcDayInWeekNumber.ValidRange))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDayInWeekNumber.ValidRange", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcDayInWeekNumber
	{
		public static readonly IfcDayInWeekNumber ValidRange = new IfcDayInWeekNumber();
		protected IfcDayInWeekNumber() {}
	}
}
