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
	public partial class IfcUShapeProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcUShapeProfileDef");

		/// <summary>
		/// Tests the express where clause ValidFlangeThickness
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidFlangeThickness() {
			var retVal = false;
			try {
				retVal = FlangeThickness < (Depth / 2);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidFlangeThickness' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause ValidWebThickness
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidWebThickness() {
			var retVal = false;
			try {
				retVal = WebThickness < FlangeWidth;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidWebThickness' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidFlangeThickness())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidFlangeThickness", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidWebThickness())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidWebThickness", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
