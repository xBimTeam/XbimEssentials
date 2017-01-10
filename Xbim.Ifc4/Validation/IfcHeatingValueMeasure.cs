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
	public partial struct IfcHeatingValueMeasure : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcHeatingValueMeasure clause) {
			var retVal = false;
			if (clause == Where.IfcHeatingValueMeasure.WR1) {
				try {
					retVal = this > 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.MeasureResource.IfcHeatingValueMeasure");
					Log.Error("Exception thrown evaluating where-clause 'IfcHeatingValueMeasure.WR1'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public  IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcHeatingValueMeasure.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcHeatingValueMeasure.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcHeatingValueMeasure
	{
		public static readonly IfcHeatingValueMeasure WR1 = new IfcHeatingValueMeasure();
		protected IfcHeatingValueMeasure() {}
	}
}
