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
	public partial struct IfcMonthInYearNumber : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcMonthInYearNumber clause) {
			var retVal = false;
			if (clause == Where.IfcMonthInYearNumber.ValidRange) {
				try {
					retVal = ((1 <= this) && (this <= 12) );
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.DateTimeResource.IfcMonthInYearNumber");
					Log.Error("Exception thrown evaluating where-clause 'IfcMonthInYearNumber.ValidRange'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public  IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcMonthInYearNumber.ValidRange))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMonthInYearNumber.ValidRange", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcMonthInYearNumber
	{
		public static readonly IfcMonthInYearNumber ValidRange = new IfcMonthInYearNumber();
		protected IfcMonthInYearNumber() {}
	}
}
