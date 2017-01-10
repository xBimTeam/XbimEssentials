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
	public partial struct IfcPHMeasure : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPHMeasure clause) {
			var retVal = false;
			if (clause == Where.IfcPHMeasure.WR21) {
				try {
					retVal = ((0 <= this) && (this <= 14) );
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.MeasureResource.IfcPHMeasure");
					Log.Error("Exception thrown evaluating where-clause 'IfcPHMeasure.WR21'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public  IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPHMeasure.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPHMeasure.WR21", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPHMeasure
	{
		public static readonly IfcPHMeasure WR21 = new IfcPHMeasure();
		protected IfcPHMeasure() {}
	}
}
