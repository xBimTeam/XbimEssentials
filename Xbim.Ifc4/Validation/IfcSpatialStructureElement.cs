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
namespace Xbim.Ifc4.ProductExtension
{
	public partial class IfcSpatialStructureElement : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcSpatialStructureElement");

		/// <summary>
		/// Tests the express where clause WR41
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR41() {
			var retVal = false;
			try {
				retVal = (HIINDEX(this/* as IfcObjectDefinition*/.Decomposes) == 1) && (TYPEOF(this/* as IfcObjectDefinition*/.Decomposes.ToArray()[0]).Contains("IFC4.IFCRELAGGREGATES")) && ((TYPEOF(this/* as IfcObjectDefinition*/.Decomposes.ToArray()[0].RelatingObject).Contains("IFC4.IFCPROJECT")) || (TYPEOF(this/* as IfcObjectDefinition*/.Decomposes.ToArray()[0].RelatingObject).Contains("IFC4.IFCSPATIALSTRUCTUREELEMENT")));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR41' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!WR41())
				yield return new ValidationResult() { Item = this, IssueSource = "WR41", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
