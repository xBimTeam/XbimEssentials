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
namespace Xbim.Ifc4.TopologyResource
{
	public partial class IfcAdvancedFace : IExpressValidatable
	{
		public enum IfcAdvancedFaceClause
		{
			ApplicableSurface,
			RequiresEdgeCurve,
			ApplicableEdgeCurves,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcAdvancedFaceClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcAdvancedFaceClause.ApplicableSurface:
						retVal = Functions.SIZEOF(Functions.NewTypesArray("IFC4.IFCELEMENTARYSURFACE", "IFC4.IFCSWEPTSURFACE", "IFC4.IFCBSPLINESURFACE") * Functions.TYPEOF(this/* as IfcFaceSurface*/.FaceSurface)) == 1;
						break;
					case IfcAdvancedFaceClause.RequiresEdgeCurve:
						retVal = Functions.SIZEOF(this/* as IfcFace*/.Bounds.Where(Bnds => Functions.TYPEOF(Bnds.Bound).Contains("IFC4.IFCEDGELOOP")).Where(ElpFbnds => !(Functions.SIZEOF(ElpFbnds.Bound.AsIfcEdgeLoop().EdgeList.Where(Oe => !(Functions.TYPEOF(Oe/* as IfcOrientedEdge*/.EdgeElement).Contains("IFC4.IFCEDGECURVE")))) == 0))) == 0;
						break;
					case IfcAdvancedFaceClause.ApplicableEdgeCurves:
						retVal = Functions.SIZEOF(this/* as IfcFace*/.Bounds.Where(Bnds => Functions.TYPEOF(Bnds.Bound).Contains("IFC4.IFCEDGELOOP")).Where(ElpFbnds => !(Functions.SIZEOF(ElpFbnds.Bound.AsIfcEdgeLoop().EdgeList.Where(Oe => !(Functions.SIZEOF(Functions.NewTypesArray("IFC4.IFCLINE", "IFC4.IFCCONIC", "IFC4.IFCPOLYLINE", "IFC4.IFCBSPLINECURVE") * Functions.TYPEOF(Oe/* as IfcOrientedEdge*/.EdgeElement.AsIfcEdgeCurve().EdgeGeometry)) == 1))) == 0))) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.TopologyResource.IfcAdvancedFace>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcAdvancedFace.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcAdvancedFaceClause.ApplicableSurface))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAdvancedFace.ApplicableSurface", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAdvancedFaceClause.RequiresEdgeCurve))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAdvancedFace.RequiresEdgeCurve", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAdvancedFaceClause.ApplicableEdgeCurves))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAdvancedFace.ApplicableEdgeCurves", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
