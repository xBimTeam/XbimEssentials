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
	public partial class IfcLShapeProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcLShapeProfileDef");

		/// <summary>
		/// Tests the express where clause ValidThickness
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidThickness() {
			var retVal = false;
			try {
				retVal = (Thickness < Depth) && (!(EXISTS(Width)) || (Thickness < Width));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidThickness' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidThickness())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidThickness", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
