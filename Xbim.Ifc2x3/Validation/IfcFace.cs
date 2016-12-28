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
	public partial class IfcFace : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.TopologyResource.IfcFace");

		/// <summary>
		/// Tests the express where clause WR1
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR1() {
			var retVal = false;
			try {
				retVal = SIZEOF(Bounds.Where(temp => TYPEOF(temp).Contains("IFC2X3.IFCFACEOUTERBOUND"))) <= 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR1' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR1())
				yield return new ValidationResult() { Item = this, IssueSource = "WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
