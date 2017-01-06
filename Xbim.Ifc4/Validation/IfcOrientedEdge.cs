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
	public partial class IfcOrientedEdge : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.TopologyResource.IfcOrientedEdge");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcOrientedEdge clause) {
			var retVal = false;
			if (clause == Where.IfcOrientedEdge.EdgeElementNotOriented) {
				try {
					retVal = !(TYPEOF(EdgeElement).Contains("IFC4.IFCORIENTEDEDGE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcOrientedEdge.EdgeElementNotOriented' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcOrientedEdge.EdgeElementNotOriented))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcOrientedEdge.EdgeElementNotOriented", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcOrientedEdge
	{
		public static readonly IfcOrientedEdge EdgeElementNotOriented = new IfcOrientedEdge();
		protected IfcOrientedEdge() {}
	}
}
