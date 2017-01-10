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
						retVal = SIZEOF(AssignedItems.Where(temp => (SIZEOF(TYPEOF(temp) * NewArray("IFC4.IFCSHAPEREPRESENTATION", "IFC4.IFCGEOMETRICREPRESENTATIONITEM", "IFC4.IFCMAPPEDITEM")) == 1))) == SIZEOF(AssignedItems);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.PresentationOrganizationResource.IfcPresentationLayerAssignment");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPresentationLayerAssignment.{0}' for #{1}.", clause,EntityLabel), ex);
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
