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
namespace Xbim.Ifc2x3.SharedFacilitiesElements
{
	public partial class IfcInventory : IExpressValidatable
	{
		public enum IfcInventoryClause
		{
			WR41,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcInventoryClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcInventoryClause.WR41:
						retVal = Functions.SIZEOF(this/* as IfcGroup*/.IsGroupedBy.RelatedObjects.Where(temp => !((Functions.TYPEOF(temp).Contains("IFC2X3.IFCSPACE")) || (Functions.TYPEOF(temp).Contains("IFC2X3.IFCASSET")) || (Functions.TYPEOF(temp).Contains("IFC2X3.IFCFURNISHINGELEMENT"))))) == 0;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.SharedFacilitiesElements.IfcInventory>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcInventory.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcInventoryClause.WR41))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcInventory.WR41", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
