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
namespace Xbim.Ifc4.MeasureResource
{
	public partial struct IfcPositiveInteger : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.MeasureResource.IfcPositiveInteger");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPositiveInteger clause) {
			var retVal = false;
			if (clause == Where.IfcPositiveInteger.WR1) {
				try {
					retVal = this > 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPositiveInteger.WR1'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPositiveInteger.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPositiveInteger.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPositiveInteger
	{
		public static readonly IfcPositiveInteger WR1 = new IfcPositiveInteger();
		protected IfcPositiveInteger() {}
	}
}
