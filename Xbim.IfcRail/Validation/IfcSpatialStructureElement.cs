using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;

// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.IfcRail.ProductExtension
{
	public partial class IfcSpatialStructureElement : IExpressValidatable
	{
		public enum IfcSpatialStructureElementClause
		{
			WR41,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcSpatialStructureElementClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcSpatialStructureElementClause.WR41:
						retVal = (Functions.HIINDEX(this/* as IfcObjectDefinition*/.Decomposes) == 1) && (Functions.TYPEOF(this/* as IfcObjectDefinition*/.Decomposes.ItemAt(0)).Contains("IFCRELAGGREGATES")) && ((Functions.TYPEOF(this/* as IfcObjectDefinition*/.Decomposes.ItemAt(0).RelatingObject).Contains("IFCPROJECT")) || (Functions.TYPEOF(this/* as IfcObjectDefinition*/.Decomposes.ItemAt(0).RelatingObject).Contains("IFCSPATIALSTRUCTUREELEMENT")));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.IfcRail.ProductExtension.IfcSpatialStructureElement>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcSpatialStructureElement.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcSpatialStructureElementClause.WR41))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSpatialStructureElement.WR41", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
