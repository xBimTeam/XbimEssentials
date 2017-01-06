using System;
using log4net;
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
using static Xbim.Ifc2x3.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.RepresentationResource
{
	public partial class IfcShapeRepresentation : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.RepresentationResource.IfcShapeRepresentation");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcShapeRepresentation clause) {
			var retVal = false;
			if (clause == Where.IfcShapeRepresentation.WR21) {
				try {
					retVal = TYPEOF(this/* as IfcRepresentation*/.ContextOfItems).Contains("IFC2X3.IFCGEOMETRICREPRESENTATIONCONTEXT");
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcShapeRepresentation.WR21' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcShapeRepresentation.WR22) {
				try {
					retVal = SIZEOF(Items.Where(temp => (TYPEOF(temp).Contains("IFC2X3.IFCTOPOLOGICALREPRESENTATIONITEM")) && (!(SIZEOF(NewArray("IFC2X3.IFCVERTEXPOINT", "IFC2X3.IFCEDGECURVE", "IFC2X3.IFCFACESURFACE") * TYPEOF(temp)) == 1)))) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcShapeRepresentation.WR22' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcShapeRepresentation.WR23) {
				try {
					retVal = EXISTS(this/* as IfcRepresentation*/.RepresentationType);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcShapeRepresentation.WR23' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcShapeRepresentation.WR24) {
				try {
					retVal = IfcShapeRepresentationTypes(this/* as IfcRepresentation*/.RepresentationType, this/* as IfcRepresentation*/.Items);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcShapeRepresentation.WR24' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcShapeModel)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcShapeRepresentation.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcShapeRepresentation.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.WR22", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcShapeRepresentation.WR23))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.WR23", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcShapeRepresentation.WR24))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShapeRepresentation.WR24", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcShapeRepresentation : IfcShapeModel
	{
		public static readonly IfcShapeRepresentation WR21 = new IfcShapeRepresentation();
		public static readonly IfcShapeRepresentation WR22 = new IfcShapeRepresentation();
		public static readonly IfcShapeRepresentation WR23 = new IfcShapeRepresentation();
		public static readonly IfcShapeRepresentation WR24 = new IfcShapeRepresentation();
		protected IfcShapeRepresentation() {}
	}
}
