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
		/// Tests the express where clause EdgeElementNotOriented
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool EdgeElementNotOriented() {
			var retVal = false;
			try {
				retVal = !(TYPEOF(EdgeElement).Contains("IFC4.IFCORIENTEDEDGE"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'EdgeElementNotOriented' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!EdgeElementNotOriented())
				yield return new ValidationResult() { Item = this, IssueSource = "EdgeElementNotOriented", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
