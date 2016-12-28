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
namespace Xbim.Ifc4.RepresentationResource
{
	public partial class IfcShapeRepresentation : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcShapeRepresentation");

		/// <summary>
		/// Tests the express where clause CorrectContext
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrectContext() {
			var retVal = false;
			try {
				retVal = TYPEOF(this/* as IfcRepresentation*/.ContextOfItems).Contains("IFC4.IFCGEOMETRICREPRESENTATIONCONTEXT");
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrectContext' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause NoTopologicalItem
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NoTopologicalItem() {
			var retVal = false;
			try {
				retVal = SIZEOF(Items.Where(temp => (TYPEOF(temp).Contains("IFC4.IFCTOPOLOGICALREPRESENTATIONITEM")) && (!(SIZEOF(NewArray("IFC4.IFCVERTEXPOINT", "IFC4.IFCEDGECURVE", "IFC4.IFCFACESURFACE") * TYPEOF(temp)) == 1)))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NoTopologicalItem' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause HasRepresentationType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool HasRepresentationType() {
			var retVal = false;
			try {
				retVal = EXISTS(this/* as IfcRepresentation*/.RepresentationType);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'HasRepresentationType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause CorrectItemsForType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrectItemsForType() {
			var retVal = false;
			try {
				retVal = IfcShapeRepresentationTypes(this/* as IfcRepresentation*/.RepresentationType, this/* as IfcRepresentation*/.Items);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrectItemsForType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause HasRepresentationIdentifier
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool HasRepresentationIdentifier() {
			var retVal = false;
			try {
				retVal = EXISTS(this/* as IfcRepresentation*/.RepresentationIdentifier);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'HasRepresentationIdentifier' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!CorrectContext())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrectContext", IssueType = ValidationFlags.EntityWhereClauses };
			if (!NoTopologicalItem())
				yield return new ValidationResult() { Item = this, IssueSource = "NoTopologicalItem", IssueType = ValidationFlags.EntityWhereClauses };
			if (!HasRepresentationType())
				yield return new ValidationResult() { Item = this, IssueSource = "HasRepresentationType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!CorrectItemsForType())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrectItemsForType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!HasRepresentationIdentifier())
				yield return new ValidationResult() { Item = this, IssueSource = "HasRepresentationIdentifier", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
