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
namespace Xbim.Ifc2x3.PresentationResource
{
	public partial struct IfcFontWeight : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationResource.IfcFontWeight");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcFontWeight clause) {
			var retVal = false;
			if (clause == Where.IfcFontWeight.WR1) {
				try {
					retVal = NewArray("normal", "small-caps", "100", "200", "300", "400", "500", "600", "700", "800", "900").Contains(this);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcFontWeight.WR1'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcFontWeight.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFontWeight.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcFontWeight
	{
		public static readonly IfcFontWeight WR1 = new IfcFontWeight();
		protected IfcFontWeight() {}
	}
}
