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
namespace Xbim.Ifc2x3.GeometricModelResource
{
	public partial class IfcBooleanClippingResult : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometricModelResource.IfcBooleanClippingResult");

		/// <summary>
		/// Tests the express where clause WR1
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR1() {
			var retVal = false;
			try {
				retVal = (TYPEOF(FirstOperand).Contains("IFC2X3.IFCSWEPTAREASOLID")) || (TYPEOF(FirstOperand).Contains("IFC2X3.IFCBOOLEANCLIPPINGRESULT"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR1' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR2
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR2() {
			var retVal = false;
			try {
				retVal = (TYPEOF(SecondOperand).Contains("IFC2X3.IFCHALFSPACESOLID"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR2' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR3
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR3() {
			var retVal = false;
			try {
				retVal = Operator == IfcBooleanOperator.DIFFERENCE;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR3' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!WR1())
				yield return new ValidationResult() { Item = this, IssueSource = "WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR2())
				yield return new ValidationResult() { Item = this, IssueSource = "WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR3())
				yield return new ValidationResult() { Item = this, IssueSource = "WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
