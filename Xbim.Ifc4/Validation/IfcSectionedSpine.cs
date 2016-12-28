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
	public partial class IfcSectionedSpine : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcSectionedSpine");

		/// <summary>
		/// Tests the express where clause CorrespondingSectionPositions
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrespondingSectionPositions() {
			var retVal = false;
			try {
				retVal = SIZEOF(CrossSections) == SIZEOF(CrossSectionPositions);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrespondingSectionPositions' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause ConsistentProfileTypes
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ConsistentProfileTypes() {
			var retVal = false;
			try {
				retVal = SIZEOF(CrossSections.Where(temp => CrossSections.ToArray()[0].ProfileType != temp.ProfileType)) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ConsistentProfileTypes' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause SpineCurveDim
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool SpineCurveDim() {
			var retVal = false;
			try {
				retVal = SpineCurve.Dim == 3;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'SpineCurveDim' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!CorrespondingSectionPositions())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrespondingSectionPositions", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ConsistentProfileTypes())
				yield return new ValidationResult() { Item = this, IssueSource = "ConsistentProfileTypes", IssueType = ValidationFlags.EntityWhereClauses };
			if (!SpineCurveDim())
				yield return new ValidationResult() { Item = this, IssueSource = "SpineCurveDim", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
