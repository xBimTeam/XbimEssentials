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
namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
	public partial class IfcTextLiteralWithExtent : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDefinitionResource.IfcTextLiteralWithExtent");

		/// <summary>
		/// Tests the express where clause WR31
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR31() {
			var retVal = false;
			try {
				retVal = !(TYPEOF(Extent).Contains("IFC2X3.IFCPLANARBOX"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR31' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR31())
				yield return new ValidationResult() { Item = this, IssueSource = "WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
