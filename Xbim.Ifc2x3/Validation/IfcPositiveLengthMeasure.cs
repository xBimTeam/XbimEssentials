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
namespace Xbim.Ifc2x3.MeasureResource
{
	public partial struct IfcPositiveLengthMeasure : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPositiveLengthMeasure clause) {
			var retVal = false;
			if (clause == Where.IfcPositiveLengthMeasure.WR1) {
				try {
					retVal = this > 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.MeasureResource.IfcPositiveLengthMeasure");
					Log.Error("Exception thrown evaluating where-clause 'IfcPositiveLengthMeasure.WR1'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public  IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPositiveLengthMeasure.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPositiveLengthMeasure.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcPositiveLengthMeasure
	{
		public static readonly IfcPositiveLengthMeasure WR1 = new IfcPositiveLengthMeasure();
		protected IfcPositiveLengthMeasure() {}
	}
}
