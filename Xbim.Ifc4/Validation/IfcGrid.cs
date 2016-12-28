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
	public partial class IfcGrid : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcGrid");

		/// <summary>
		/// Tests the express where clause HasPlacement
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool HasPlacement() {
			var retVal = false;
			try {
				retVal = EXISTS(this/* as IfcProduct*/.ObjectPlacement);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'HasPlacement' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!HasPlacement())
				yield return new ValidationResult() { Item = this, IssueSource = "HasPlacement", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
