using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.ProductExtension
{
	public partial class IfcZone : IExpressValidatable
	{
		public enum IfcZoneClause
		{
			WR1,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcZoneClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcZoneClause.WR1:
						retVal = (Functions.SIZEOF(this/* as IfcGroup*/.IsGroupedBy) == 0) || (Functions.SIZEOF(this/* as IfcGroup*/.IsGroupedBy.ItemAt(0).RelatedObjects.Where(temp => !((Functions.TYPEOF(temp).Contains("IFC4.IFCZONE")) || (Functions.TYPEOF(temp).Contains("IFC4.IFCSPACE")) || (Functions.TYPEOF(temp).Contains("IFC4.IFCSPATIALZONE"))))) == 0);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProductExtension.IfcZone>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcZone.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcZoneClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcZone.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
