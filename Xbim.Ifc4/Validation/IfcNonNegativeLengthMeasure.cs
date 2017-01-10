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
	public partial struct IfcNonNegativeLengthMeasure : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcNonNegativeLengthMeasure clause) {
			var retVal = false;
			if (clause == Where.IfcNonNegativeLengthMeasure.NotNegative) {
				try {
					retVal = this >= 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.MeasureResource.IfcNonNegativeLengthMeasure");
					Log.Error("Exception thrown evaluating where-clause 'IfcNonNegativeLengthMeasure.NotNegative'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public  IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcNonNegativeLengthMeasure.NotNegative))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcNonNegativeLengthMeasure.NotNegative", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcNonNegativeLengthMeasure
	{
		public static readonly IfcNonNegativeLengthMeasure NotNegative = new IfcNonNegativeLengthMeasure();
		protected IfcNonNegativeLengthMeasure() {}
	}
}
