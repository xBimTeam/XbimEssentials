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
namespace Xbim.Ifc4.ProductExtension
{
	public partial class IfcGrid : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcGrid");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcGrid clause) {
			var retVal = false;
			if (clause == Where.IfcGrid.HasPlacement) {
				try {
					retVal = EXISTS(this/* as IfcProduct*/.ObjectPlacement);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcGrid.HasPlacement' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcProduct)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcGrid.HasPlacement))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGrid.HasPlacement", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcGrid : IfcProduct
	{
		public static readonly IfcGrid HasPlacement = new IfcGrid();
		protected IfcGrid() {}
	}
}
