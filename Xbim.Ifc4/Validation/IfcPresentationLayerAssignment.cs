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
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationOrganizationResource.IfcPresentationLayerAssignment");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPresentationLayerAssignment clause) {
			var retVal = false;
			if (clause == Where.IfcPresentationLayerAssignment.ApplicableItems) {
				try {
					retVal = SIZEOF(AssignedItems.Where(temp => (SIZEOF(TYPEOF(temp) * NewArray("IFC4.IFCSHAPEREPRESENTATION", "IFC4.IFCGEOMETRICREPRESENTATIONITEM", "IFC4.IFCMAPPEDITEM")) == 1))) == SIZEOF(AssignedItems);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPresentationLayerAssignment.ApplicableItems' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPresentationLayerAssignment.ApplicableItems))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPresentationLayerAssignment.ApplicableItems", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPresentationLayerAssignment
	{
		public static readonly IfcPresentationLayerAssignment ApplicableItems = new IfcPresentationLayerAssignment();
		protected IfcPresentationLayerAssignment() {}
	}
}
