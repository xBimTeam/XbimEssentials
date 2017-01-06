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
	public partial class IfcTopologyRepresentation : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcTopologyRepresentation");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTopologyRepresentation clause) {
			var retVal = false;
			if (clause == Where.IfcTopologyRepresentation.WR21) {
				try {
					retVal = SIZEOF(this/* as IfcRepresentation*/.Items.Where(temp => !(TYPEOF(temp).Contains("IFC4.IFCTOPOLOGICALREPRESENTATIONITEM")))) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcTopologyRepresentation.WR21' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcTopologyRepresentation.WR22) {
				try {
					retVal = EXISTS(this/* as IfcRepresentation*/.RepresentationType);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcTopologyRepresentation.WR22' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcTopologyRepresentation.WR23) {
				try {
					retVal = IfcTopologyRepresentationTypes(this/* as IfcRepresentation*/.RepresentationType, this/* as IfcRepresentation*/.Items);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcTopologyRepresentation.WR23' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcTopologyRepresentation.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTopologyRepresentation.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcTopologyRepresentation.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTopologyRepresentation.WR22", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcTopologyRepresentation.WR23))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTopologyRepresentation.WR23", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcTopologyRepresentation : IfcShapeModel
	{
		public static readonly IfcTopologyRepresentation WR21 = new IfcTopologyRepresentation();
		public static readonly IfcTopologyRepresentation WR22 = new IfcTopologyRepresentation();
		public static readonly IfcTopologyRepresentation WR23 = new IfcTopologyRepresentation();
		protected IfcTopologyRepresentation() {}
	}
}
