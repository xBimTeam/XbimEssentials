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
	public partial struct IfcMonthInYearNumber : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.DateTimeResource.IfcMonthInYearNumber");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcMonthInYearNumber clause) {
			var retVal = false;
			if (clause == Where.IfcMonthInYearNumber.WR1) {
				try {
					retVal = ((1 <= this) && (this <= 12) );
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcMonthInYearNumber.WR1'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcMonthInYearNumber.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMonthInYearNumber.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcMonthInYearNumber
	{
		public static readonly IfcMonthInYearNumber WR1 = new IfcMonthInYearNumber();
		protected IfcMonthInYearNumber() {}
	}
}
