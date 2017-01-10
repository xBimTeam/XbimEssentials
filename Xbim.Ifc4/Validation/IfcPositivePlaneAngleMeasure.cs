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
	public partial struct IfcPositivePlaneAngleMeasure : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPositivePlaneAngleMeasure clause) {
			var retVal = false;
			if (clause == Where.IfcPositivePlaneAngleMeasure.WR1) {
				try {
					retVal = this > 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.MeasureResource.IfcPositivePlaneAngleMeasure");
					Log.Error("Exception thrown evaluating where-clause 'IfcPositivePlaneAngleMeasure.WR1'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public  IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPositivePlaneAngleMeasure.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPositivePlaneAngleMeasure.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPositivePlaneAngleMeasure
	{
		public static readonly IfcPositivePlaneAngleMeasure WR1 = new IfcPositivePlaneAngleMeasure();
		protected IfcPositivePlaneAngleMeasure() {}
	}
}
