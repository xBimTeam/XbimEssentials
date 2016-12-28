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
namespace Xbim.Ifc4.RepresentationResource
{
	public partial class IfcGeometricRepresentationSubContext : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcGeometricRepresentationSubContext");

		/// <summary>
		/// Tests the express where clause ParentNoSub
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ParentNoSub() {
			var retVal = false;
			try {
				retVal = !(TYPEOF(ParentContext).Contains("IFC4.IFCGEOMETRICREPRESENTATIONSUBCONTEXT"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ParentNoSub' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause UserTargetProvided
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool UserTargetProvided() {
			var retVal = false;
			try {
				retVal = (TargetView != IfcGeometricProjectionEnum.USERDEFINED) || ((TargetView == IfcGeometricProjectionEnum.USERDEFINED) && EXISTS(UserDefinedTargetView));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'UserTargetProvided' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause NoCoordOperation
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NoCoordOperation() {
			var retVal = false;
			try {
				retVal = SIZEOF(this/* as IfcGeometricRepresentationContext*/.HasCoordinateOperation) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NoCoordOperation' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ParentNoSub())
				yield return new ValidationResult() { Item = this, IssueSource = "ParentNoSub", IssueType = ValidationFlags.EntityWhereClauses };
			if (!UserTargetProvided())
				yield return new ValidationResult() { Item = this, IssueSource = "UserTargetProvided", IssueType = ValidationFlags.EntityWhereClauses };
			if (!NoCoordOperation())
				yield return new ValidationResult() { Item = this, IssueSource = "NoCoordOperation", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
