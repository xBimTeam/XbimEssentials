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
	public partial struct IfcDayInMonthNumber : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.DateTimeResource.IfcDayInMonthNumber");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcDayInMonthNumber clause) {
			var retVal = false;
			if (clause == Where.IfcDayInMonthNumber.ValidRange) {
				try {
					retVal = ((1 <= this) && (this <= 31) );
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcDayInMonthNumber.ValidRange'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcDayInMonthNumber.ValidRange))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDayInMonthNumber.ValidRange", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcDayInMonthNumber
	{
		public static readonly IfcDayInMonthNumber ValidRange = new IfcDayInMonthNumber();
		protected IfcDayInMonthNumber() {}
	}
}
