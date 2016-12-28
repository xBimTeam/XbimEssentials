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
	public partial class IfcFeatureElementSubtraction : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcFeatureElementSubtraction");

		/// <summary>
		/// Tests the express where clause HasNoSubtraction
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool HasNoSubtraction() {
			var retVal = false;
			try {
				retVal = SIZEOF(this/* as IfcElement*/.HasOpenings) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'HasNoSubtraction' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause IsNotFilling
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool IsNotFilling() {
			var retVal = false;
			try {
				retVal = SIZEOF(this/* as IfcElement*/.FillsVoids) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'IsNotFilling' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!HasNoSubtraction())
				yield return new ValidationResult() { Item = this, IssueSource = "HasNoSubtraction", IssueType = ValidationFlags.EntityWhereClauses };
			if (!IsNotFilling())
				yield return new ValidationResult() { Item = this, IssueSource = "IsNotFilling", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
