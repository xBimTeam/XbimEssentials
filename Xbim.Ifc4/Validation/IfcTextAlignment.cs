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
namespace Xbim.Ifc4.PresentationAppearanceResource
{
	public partial struct IfcTextAlignment : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcTextAlignment");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTextAlignment clause) {
			var retVal = false;
			if (clause == Where.IfcTextAlignment.WR1) {
				try {
					retVal = NewArray("left", "right", "center", "justify").Contains(this);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcTextAlignment.WR1'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcTextAlignment.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTextAlignment.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcTextAlignment
	{
		public static readonly IfcTextAlignment WR1 = new IfcTextAlignment();
		protected IfcTextAlignment() {}
	}
}
