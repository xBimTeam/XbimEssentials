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
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.TopologyResource.IfcAdvancedFace");

		/// <summary>
		/// Tests the express where clause ApplicableSurface
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ApplicableSurface() {
			var retVal = false;
			try {
				retVal = SIZEOF(NewArray("IFC4.IFCELEMENTARYSURFACE", "IFC4.IFCSWEPTSURFACE", "IFC4.IFCBSPLINESURFACE") * TYPEOF(this/* as IfcFaceSurface*/.FaceSurface)) == 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ApplicableSurface' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause RequiresEdgeCurve
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool RequiresEdgeCurve() {
			var retVal = false;
			try {
				retVal = SIZEOF(this/* as IfcFace*/.Bounds.Where(Bnds => TYPEOF(Bnds.Bound).Contains("IFC4.IFCEDGELOOP")).Where(ElpFbnds => !(SIZEOF(ElpFbnds.Bound.AsIfcEdgeLoop().EdgeList.Where(Oe => !(TYPEOF(Oe/* as IfcOrientedEdge*/.EdgeElement).Contains("IFC4.IFCEDGECURVE")))) == 0))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'RequiresEdgeCurve' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause ApplicableEdgeCurves
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ApplicableEdgeCurves() {
			var retVal = false;
			try {
				retVal = SIZEOF(this/* as IfcFace*/.Bounds.Where(Bnds => TYPEOF(Bnds.Bound).Contains("IFC4.IFCEDGELOOP")).Where(ElpFbnds => !(SIZEOF(ElpFbnds.Bound.AsIfcEdgeLoop().EdgeList.Where(Oe => !(SIZEOF(NewArray("IFC4.IFCLINE", "IFC4.IFCCONIC", "IFC4.IFCPOLYLINE", "IFC4.IFCBSPLINECURVE") * TYPEOF(Oe/* as IfcOrientedEdge*/.EdgeElement.AsIfcEdgeCurve().EdgeGeometry)) == 1))) == 0))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ApplicableEdgeCurves' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ApplicableSurface())
				yield return new ValidationResult() { Item = this, IssueSource = "ApplicableSurface", IssueType = ValidationFlags.EntityWhereClauses };
			if (!RequiresEdgeCurve())
				yield return new ValidationResult() { Item = this, IssueSource = "RequiresEdgeCurve", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ApplicableEdgeCurves())
				yield return new ValidationResult() { Item = this, IssueSource = "ApplicableEdgeCurves", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
