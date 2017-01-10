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
namespace Xbim.Ifc2x3.GeometryResource
{
	public partial class IfcSweptSurface : IExpressValidatable
	{
		public enum IfcSweptSurfaceClause
		{
			WR1,
			WR2,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcSweptSurfaceClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcSweptSurfaceClause.WR1:
						retVal = !(TYPEOF(SweptCurve).Contains("IFC2X3.IFCDERIVEDPROFILEDEF"));
						break;
					case IfcSweptSurfaceClause.WR2:
						retVal = SweptCurve.ProfileType == IfcProfileTypeEnum.CURVE;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcSweptSurface");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSweptSurface.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcSweptSurfaceClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptSurface.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSweptSurfaceClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptSurface.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
