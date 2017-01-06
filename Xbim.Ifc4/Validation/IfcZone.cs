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
	public partial class IfcZone : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcZone");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcZone clause) {
			var retVal = false;
			if (clause == Where.IfcZone.WR1) {
				try {
					retVal = (SIZEOF(this/* as IfcGroup*/.IsGroupedBy) == 0) || (SIZEOF(this/* as IfcGroup*/.IsGroupedBy.ToArray()[0].RelatedObjects.Where(temp => !((TYPEOF(temp).Contains("IFC4.IFCZONE")) || (TYPEOF(temp).Contains("IFC4.IFCSPACE")) || (TYPEOF(temp).Contains("IFC4.IFCSPATIALZONE"))))) == 0);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcZone.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcObject)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcZone.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcZone.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcZone : IfcObject
	{
		public static readonly IfcZone WR1 = new IfcZone();
		protected IfcZone() {}
	}
}
