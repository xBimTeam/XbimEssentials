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
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcGeometricRepresentationSubContext clause) {
			var retVal = false;
			if (clause == Where.IfcGeometricRepresentationSubContext.ParentNoSub) {
				try {
					retVal = !(TYPEOF(ParentContext).Contains("IFC4.IFCGEOMETRICREPRESENTATIONSUBCONTEXT"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcGeometricRepresentationSubContext.ParentNoSub' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcGeometricRepresentationSubContext.UserTargetProvided) {
				try {
					retVal = (TargetView != IfcGeometricProjectionEnum.USERDEFINED) || ((TargetView == IfcGeometricProjectionEnum.USERDEFINED) && EXISTS(UserDefinedTargetView));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcGeometricRepresentationSubContext.UserTargetProvided' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcGeometricRepresentationSubContext.NoCoordOperation) {
				try {
					retVal = SIZEOF(this/* as IfcGeometricRepresentationContext*/.HasCoordinateOperation) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcGeometricRepresentationSubContext.NoCoordOperation' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcGeometricRepresentationContext)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcGeometricRepresentationSubContext.ParentNoSub))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGeometricRepresentationSubContext.ParentNoSub", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcGeometricRepresentationSubContext.UserTargetProvided))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGeometricRepresentationSubContext.UserTargetProvided", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcGeometricRepresentationSubContext.NoCoordOperation))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGeometricRepresentationSubContext.NoCoordOperation", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcGeometricRepresentationSubContext : IfcGeometricRepresentationContext
	{
		public static readonly IfcGeometricRepresentationSubContext ParentNoSub = new IfcGeometricRepresentationSubContext();
		public static readonly IfcGeometricRepresentationSubContext UserTargetProvided = new IfcGeometricRepresentationSubContext();
		public static readonly IfcGeometricRepresentationSubContext NoCoordOperation = new IfcGeometricRepresentationSubContext();
		protected IfcGeometricRepresentationSubContext() {}
	}
}
