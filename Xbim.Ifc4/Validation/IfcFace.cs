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
	public partial class IfcFace : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.TopologyResource.IfcFace");

		/// <summary>
		/// Tests the express where clause HasOuterBound
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool HasOuterBound() {
			var retVal = false;
			try {
				retVal = SIZEOF(Bounds.Where(temp => TYPEOF(temp).Contains("IFC4.IFCFACEOUTERBOUND"))) <= 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'HasOuterBound' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!HasOuterBound())
				yield return new ValidationResult() { Item = this, IssueSource = "HasOuterBound", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
