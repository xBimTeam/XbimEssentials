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
namespace Xbim.Ifc4.RepresentationResource
{
	public partial class IfcStyledRepresentation : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcStyledRepresentation");

		/// <summary>
		/// Tests the express where clause OnlyStyledItems
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool OnlyStyledItems() {
			var retVal = false;
			try {
				retVal = SIZEOF(this/* as IfcRepresentation*/.Items.Where(temp => (!(TYPEOF(temp).Contains("IFC4.IFCSTYLEDITEM"))))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'OnlyStyledItems' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!OnlyStyledItems())
				yield return new ValidationResult() { Item = this, IssueSource = "OnlyStyledItems", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
