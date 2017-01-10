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
namespace Xbim.Ifc2x3.RepresentationResource
{
	public partial class IfcGeometricRepresentationSubContext : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcGeometricRepresentationSubContext clause) {
			var retVal = false;
			if (clause == Where.IfcGeometricRepresentationSubContext.WR31) {
				try {
					retVal = !(TYPEOF(ParentContext).Contains("IFC2X3.IFCGEOMETRICREPRESENTATIONSUBCONTEXT"));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.RepresentationResource.IfcGeometricRepresentationSubContext");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcGeometricRepresentationSubContext.WR31' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcGeometricRepresentationSubContext.WR32) {
				try {
					retVal = (TargetView != IfcGeometricProjectionEnum.USERDEFINED) || ((TargetView == IfcGeometricProjectionEnum.USERDEFINED) && EXISTS(UserDefinedTargetView));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.RepresentationResource.IfcGeometricRepresentationSubContext");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcGeometricRepresentationSubContext.WR32' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcGeometricRepresentationSubContext.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGeometricRepresentationSubContext.WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcGeometricRepresentationSubContext.WR32))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGeometricRepresentationSubContext.WR32", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcGeometricRepresentationSubContext
	{
		public static readonly IfcGeometricRepresentationSubContext WR31 = new IfcGeometricRepresentationSubContext();
		public static readonly IfcGeometricRepresentationSubContext WR32 = new IfcGeometricRepresentationSubContext();
		protected IfcGeometricRepresentationSubContext() {}
	}
}
