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
	public partial class IfcBSplineSurfaceWithKnots : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcBSplineSurfaceWithKnots");

		/// <summary>
		/// Tests the express where clause UDirectionConstraints
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool UDirectionConstraints() {
			var retVal = false;
			try {
				retVal = IfcConstraintsParamBSpline(this/* as IfcBSplineSurface*/.UDegree, KnotUUpper, this/* as IfcBSplineSurface*/.UUpper, UMultiplicities, UKnots);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'UDirectionConstraints' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause VDirectionConstraints
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool VDirectionConstraints() {
			var retVal = false;
			try {
				retVal = IfcConstraintsParamBSpline(this/* as IfcBSplineSurface*/.VDegree, KnotVUpper, this/* as IfcBSplineSurface*/.VUpper, VMultiplicities, VKnots);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'VDirectionConstraints' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause CorrespondingULists
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrespondingULists() {
			var retVal = false;
			try {
				retVal = SIZEOF(UMultiplicities) == KnotUUpper;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrespondingULists' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause CorrespondingVLists
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrespondingVLists() {
			var retVal = false;
			try {
				retVal = SIZEOF(VMultiplicities) == KnotVUpper;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrespondingVLists' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!UDirectionConstraints())
				yield return new ValidationResult() { Item = this, IssueSource = "UDirectionConstraints", IssueType = ValidationFlags.EntityWhereClauses };
			if (!VDirectionConstraints())
				yield return new ValidationResult() { Item = this, IssueSource = "VDirectionConstraints", IssueType = ValidationFlags.EntityWhereClauses };
			if (!CorrespondingULists())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrespondingULists", IssueType = ValidationFlags.EntityWhereClauses };
			if (!CorrespondingVLists())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrespondingVLists", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
