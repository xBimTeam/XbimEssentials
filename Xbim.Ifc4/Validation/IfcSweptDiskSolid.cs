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
namespace Xbim.Ifc4.GeometricModelResource
{
	public partial class IfcSweptDiskSolid : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcSweptDiskSolid");

		/// <summary>
		/// Tests the express where clause DirectrixDim
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool DirectrixDim() {
			var retVal = false;
			try {
				retVal = Directrix.Dim == 3;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'DirectrixDim' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause InnerRadiusSize
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool InnerRadiusSize() {
			var retVal = false;
			try {
				retVal = (!EXISTS(InnerRadius)) || (Radius > InnerRadius);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'InnerRadiusSize' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause DirectrixBounded
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool DirectrixBounded() {
			var retVal = false;
			try {
				retVal = (EXISTS(StartParam) && EXISTS(EndParam)) || (SIZEOF(NewArray("IFC4.IFCCONIC", "IFC4.IFCBOUNDEDCURVE") * TYPEOF(Directrix)) == 1);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'DirectrixBounded' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!DirectrixDim())
				yield return new ValidationResult() { Item = this, IssueSource = "DirectrixDim", IssueType = ValidationFlags.EntityWhereClauses };
			if (!InnerRadiusSize())
				yield return new ValidationResult() { Item = this, IssueSource = "InnerRadiusSize", IssueType = ValidationFlags.EntityWhereClauses };
			if (!DirectrixBounded())
				yield return new ValidationResult() { Item = this, IssueSource = "DirectrixBounded", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
