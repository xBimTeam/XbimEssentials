using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
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
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.ProductExtension
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
						retVal = Functions.SIZEOF(this/* as IfcGroup*/.IsGroupedBy.RelatedObjects.Where(temp => !((Functions.TYPEOF(temp).Contains("IFC2X3.IFCZONE")) || (Functions.TYPEOF(temp).Contains("IFC2X3.IFCSPACE"))))) == 0;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.ProductExtension.IfcZone>();
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
