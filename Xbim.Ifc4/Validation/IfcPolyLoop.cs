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
	public partial class IfcPolyLoop : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.TopologyResource.IfcPolyLoop");

		/// <summary>
		/// Tests the express where clause AllPointsSameDim
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool AllPointsSameDim() {
			var retVal = false;
			try {
				retVal = SIZEOF(Polygon.Where(Temp => Temp.Dim != Polygon.ToArray()[0].Dim)) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'AllPointsSameDim' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!AllPointsSameDim())
				yield return new ValidationResult() { Item = this, IssueSource = "AllPointsSameDim", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
