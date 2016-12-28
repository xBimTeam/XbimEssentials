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
namespace Xbim.Ifc4.ProfileResource
{
	public partial class IfcRoundedRectangleProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcRoundedRectangleProfileDef");

		/// <summary>
		/// Tests the express where clause ValidRadius
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidRadius() {
			var retVal = false;
			try {
				retVal = ((RoundingRadius <= (this/* as IfcRectangleProfileDef*/.XDim / 2)) && (RoundingRadius <= (this/* as IfcRectangleProfileDef*/.YDim / 2)));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidRadius' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidRadius())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidRadius", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
