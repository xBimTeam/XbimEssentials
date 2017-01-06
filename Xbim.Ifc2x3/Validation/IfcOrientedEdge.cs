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
namespace Xbim.Ifc2x3.TopologyResource
{
	public partial class IfcOrientedEdge : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.TopologyResource.IfcOrientedEdge");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcOrientedEdge clause) {
			var retVal = false;
			if (clause == Where.IfcOrientedEdge.WR1) {
				try {
					retVal = !(TYPEOF(EdgeElement).Contains("IFC2X3.IFCORIENTEDEDGE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcOrientedEdge.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcOrientedEdge.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcOrientedEdge.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcOrientedEdge
	{
		public static readonly IfcOrientedEdge WR1 = new IfcOrientedEdge();
		protected IfcOrientedEdge() {}
	}
}
