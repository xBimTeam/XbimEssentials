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
namespace Xbim.Ifc4.GeometryResource
{
	public partial class IfcVector : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcVector");

		/// <summary>
		/// Tests the express where clause MagGreaterOrEqualZero
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MagGreaterOrEqualZero() {
			var retVal = false;
			try {
				retVal = Magnitude >= 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MagGreaterOrEqualZero' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!MagGreaterOrEqualZero())
				yield return new ValidationResult() { Item = this, IssueSource = "MagGreaterOrEqualZero", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
