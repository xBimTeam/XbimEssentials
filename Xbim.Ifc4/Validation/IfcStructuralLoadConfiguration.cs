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
namespace Xbim.Ifc4.StructuralLoadResource
{
	public partial class IfcStructuralLoadConfiguration : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralLoadResource.IfcStructuralLoadConfiguration");

		/// <summary>
		/// Tests the express where clause ValidListSize
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidListSize() {
			var retVal = false;
			try {
				retVal = !EXISTS(Locations) || (SIZEOF(Locations) == SIZEOF(Values));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidListSize' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidListSize())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidListSize", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
