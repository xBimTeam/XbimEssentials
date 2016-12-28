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
namespace Xbim.Ifc2x3.Kernel
{
	public partial class IfcTypeProduct : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.Kernel.IfcTypeProduct");

		/// <summary>
		/// Tests the express where clause WR41
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR41() {
			var retVal = false;
			try {
				retVal = !(EXISTS(this/* as IfcTypeObject*/.ObjectTypeOf.ToArray()[0])) || (SIZEOF(this/* as IfcTypeObject*/.ObjectTypeOf.ToArray()[0].RelatedObjects.Where(temp => !(TYPEOF(temp).Contains("IFC2X3.IFCPRODUCT")))) == 0);
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
