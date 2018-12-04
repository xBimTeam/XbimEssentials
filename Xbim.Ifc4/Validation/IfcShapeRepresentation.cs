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
namespace Xbim.Ifc4.RepresentationResource
{
	public partial class IfcShapeRepresentation : IExpressValidatable
	{
		public enum IfcShapeRepresentationClause
		{
			CorrectContext,
			NoTopologicalItem,
			HasRepresentationType,
			CorrectItemsForType,
			HasRepresentationIdentifier,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcShapeRepresentationClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcShapeRepresentationClause.CorrectContext:
						retVal = Functions.TYPEOF(this/* as IfcRepresentation*/.ContextOfItems).Contains("IFC4.IFCGEOMETRICREPRESENTATIONCONTEXT");
						break;
					case IfcShapeRepresentationClause.NoTopologicalItem:
						retVal = Functions.SIZEOF(Items.Where(temp => (Functions.TYPEOF(temp).Contains("IFC4.IFCTOPOLOGICALREPRESENTATIONITEM")) && (!(Functions.SIZEOF(Functions.NewTypesArray("IFC4.IFCVERTEXPOINT", "IFC4.IFCEDGECURVE", "IFC4.IFCFACESURFACE") * Functions.TYPEOF(temp)) == 1)))) == 0;
						break;
					case IfcShapeRepresentationClause.HasRepresentationType:
						retVal = Functions.EXISTS(this/* as IfcRepresentation*/.RepresentationType);
						break;
					case IfcShapeRepresentationClause.CorrectItemsForType:
						retVal = Functions.IfcShapeRepresentationTypes(this/* as IfcRepresentation*/.RepresentationType, this/* as IfcRepresentation*/.Items);
						break;
					case IfcShapeRepresentationClause.HasRepresentationIdentifier:
						retVal = Functions.EXISTS(this/* as IfcRepresentation*/.RepresentationIdentifier);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.RepresentationResource.IfcShapeRepresentation>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcShapeRepresentation.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcShapeRepresentationClause.CorrectContext))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.CorrectContext", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcShapeRepresentationClause.NoTopologicalItem))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.NoTopologicalItem", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcShapeRepresentationClause.HasRepresentationType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.HasRepresentationType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcShapeRepresentationClause.CorrectItemsForType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.CorrectItemsForType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcShapeRepresentationClause.HasRepresentationIdentifier))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.HasRepresentationIdentifier", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
