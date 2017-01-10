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
						retVal = SIZEOF(NewArray("IFC4.IFCELEMENTARYSURFACE", "IFC4.IFCSWEPTSURFACE", "IFC4.IFCBSPLINESURFACE") * TYPEOF(this/* as IfcFaceSurface*/.FaceSurface)) == 1;
						break;
					case IfcAdvancedFaceClause.RequiresEdgeCurve:
						retVal = SIZEOF(this/* as IfcFace*/.Bounds.Where(Bnds => TYPEOF(Bnds.Bound).Contains("IFC4.IFCEDGELOOP")).Where(ElpFbnds => !(SIZEOF(ElpFbnds.Bound.AsIfcEdgeLoop().EdgeList.Where(Oe => !(TYPEOF(Oe/* as IfcOrientedEdge*/.EdgeElement).Contains("IFC4.IFCEDGECURVE")))) == 0))) == 0;
						break;
					case IfcAdvancedFaceClause.ApplicableEdgeCurves:
						retVal = SIZEOF(this/* as IfcFace*/.Bounds.Where(Bnds => TYPEOF(Bnds.Bound).Contains("IFC4.IFCEDGELOOP")).Where(ElpFbnds => !(SIZEOF(ElpFbnds.Bound.AsIfcEdgeLoop().EdgeList.Where(Oe => !(SIZEOF(NewArray("IFC4.IFCLINE", "IFC4.IFCCONIC", "IFC4.IFCPOLYLINE", "IFC4.IFCBSPLINECURVE") * TYPEOF(Oe/* as IfcOrientedEdge*/.EdgeElement.AsIfcEdgeCurve().EdgeGeometry)) == 1))) == 0))) == 0;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.TopologyResource.IfcAdvancedFace");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAdvancedFace.{0}' for #{1}.", clause,EntityLabel), ex);
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
