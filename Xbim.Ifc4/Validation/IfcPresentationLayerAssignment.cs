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
namespace Xbim.Ifc4.PresentationOrganizationResource
{
	public partial class IfcPresentationLayerAssignment : IExpressValidatable
	{
		public enum IfcPresentationLayerAssignmentClause
		{
			ApplicableItems,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcPresentationLayerAssignmentClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcPresentationLayerAssignmentClause.ApplicableItems:
						retVal = Functions.SIZEOF(AssignedItems.Where(temp => (Functions.SIZEOF(Functions.TYPEOF(temp) * Functions.NewTypesArray("IFC4.IFCSHAPEREPRESENTATION", "IFC4.IFCGEOMETRICREPRESENTATIONITEM", "IFC4.IFCMAPPEDITEM")) == 1))) == Functions.SIZEOF(AssignedItems);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.PresentationOrganizationResource.IfcPresentationLayerAssignment>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcPresentationLayerAssignment.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcPresentationLayerAssignmentClause.ApplicableItems))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPresentationLayerAssignment.ApplicableItems", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
