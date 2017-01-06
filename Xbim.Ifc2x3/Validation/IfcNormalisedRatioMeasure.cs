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
	public partial struct IfcNormalisedRatioMeasure : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.MeasureResource.IfcNormalisedRatioMeasure");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcNormalisedRatioMeasure clause) {
			var retVal = false;
			if (clause == Where.IfcNormalisedRatioMeasure.WR1) {
				try {
					retVal = ((0 <= this) && (this <= 1) );
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcNormalisedRatioMeasure.WR1'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcNormalisedRatioMeasure.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcNormalisedRatioMeasure.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcNormalisedRatioMeasure
	{
		public static readonly IfcNormalisedRatioMeasure WR1 = new IfcNormalisedRatioMeasure();
		protected IfcNormalisedRatioMeasure() {}
	}
}
