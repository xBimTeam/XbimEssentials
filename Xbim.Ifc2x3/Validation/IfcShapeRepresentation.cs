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
namespace Xbim.Ifc2x3.RepresentationResource
{
	public partial class IfcShapeRepresentation : IExpressValidatable
	{
		public enum IfcShapeRepresentationClause
		{
			WR21,
			WR22,
			WR23,
			WR24,
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
					case IfcShapeRepresentationClause.WR21:
						retVal = Functions.TYPEOF(this/* as IfcRepresentation*/.ContextOfItems).Contains("IFC2X3.IFCGEOMETRICREPRESENTATIONCONTEXT");
						break;
					case IfcShapeRepresentationClause.WR22:
						retVal = Functions.SIZEOF(Items.Where(temp => (Functions.TYPEOF(temp).Contains("IFC2X3.IFCTOPOLOGICALREPRESENTATIONITEM")) && (!(Functions.SIZEOF(Functions.NewArray("IFC2X3.IFCVERTEXPOINT", "IFC2X3.IFCEDGECURVE", "IFC2X3.IFCFACESURFACE") * Functions.TYPEOF(temp)) == 1)))) == 0;
						break;
					case IfcShapeRepresentationClause.WR23:
						retVal = Functions.EXISTS(this/* as IfcRepresentation*/.RepresentationType);
						break;
					case IfcShapeRepresentationClause.WR24:
						retVal = Functions.IfcShapeRepresentationTypes(this/* as IfcRepresentation*/.RepresentationType, this/* as IfcRepresentation*/.Items);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.RepresentationResource.IfcShapeRepresentation>();
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
			if (!ValidateClause(IfcShapeRepresentationClause.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcShapeRepresentationClause.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.WR22", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcShapeRepresentationClause.WR23))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.WR23", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcShapeRepresentationClause.WR24))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.WR24", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
