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

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcShapeRepresentation clause) {
			var retVal = false;
			if (clause == Where.IfcShapeRepresentation.CorrectContext) {
				try {
					retVal = TYPEOF(this/* as IfcRepresentation*/.ContextOfItems).Contains("IFC4.IFCGEOMETRICREPRESENTATIONCONTEXT");
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcShapeRepresentation");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcShapeRepresentation.CorrectContext' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcShapeRepresentation.NoTopologicalItem) {
				try {
					retVal = SIZEOF(Items.Where(temp => (TYPEOF(temp).Contains("IFC4.IFCTOPOLOGICALREPRESENTATIONITEM")) && (!(SIZEOF(NewArray("IFC4.IFCVERTEXPOINT", "IFC4.IFCEDGECURVE", "IFC4.IFCFACESURFACE") * TYPEOF(temp)) == 1)))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcShapeRepresentation");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcShapeRepresentation.NoTopologicalItem' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcShapeRepresentation.HasRepresentationType) {
				try {
					retVal = EXISTS(this/* as IfcRepresentation*/.RepresentationType);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcShapeRepresentation");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcShapeRepresentation.HasRepresentationType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcShapeRepresentation.CorrectItemsForType) {
				try {
					retVal = IfcShapeRepresentationTypes(this/* as IfcRepresentation*/.RepresentationType, this/* as IfcRepresentation*/.Items);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcShapeRepresentation");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcShapeRepresentation.CorrectItemsForType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcShapeRepresentation.HasRepresentationIdentifier) {
				try {
					retVal = EXISTS(this/* as IfcRepresentation*/.RepresentationIdentifier);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcShapeRepresentation");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcShapeRepresentation.HasRepresentationIdentifier' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcShapeModel)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcShapeRepresentation.CorrectContext))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.CorrectContext", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcShapeRepresentation.NoTopologicalItem))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.NoTopologicalItem", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcShapeRepresentation.HasRepresentationType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.HasRepresentationType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcShapeRepresentation.CorrectItemsForType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.CorrectItemsForType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcShapeRepresentation.HasRepresentationIdentifier))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.HasRepresentationIdentifier", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcShapeRepresentation : IfcShapeModel
	{
		public static readonly IfcShapeRepresentation CorrectContext = new IfcShapeRepresentation();
		public static readonly IfcShapeRepresentation NoTopologicalItem = new IfcShapeRepresentation();
		public static readonly IfcShapeRepresentation HasRepresentationType = new IfcShapeRepresentation();
		public static readonly IfcShapeRepresentation CorrectItemsForType = new IfcShapeRepresentation();
		public static readonly IfcShapeRepresentation HasRepresentationIdentifier = new IfcShapeRepresentation();
		protected IfcShapeRepresentation() {}
	}
}
