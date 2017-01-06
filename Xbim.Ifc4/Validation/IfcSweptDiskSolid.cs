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
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSweptDiskSolid clause) {
			var retVal = false;
			if (clause == Where.IfcSweptDiskSolid.DirectrixDim) {
				try {
					retVal = Directrix.Dim == 3;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSweptDiskSolid.DirectrixDim' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSweptDiskSolid.InnerRadiusSize) {
				try {
					retVal = (!EXISTS(InnerRadius)) || (Radius > InnerRadius);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSweptDiskSolid.InnerRadiusSize' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSweptDiskSolid.DirectrixBounded) {
				try {
					retVal = (EXISTS(StartParam) && EXISTS(EndParam)) || (SIZEOF(NewArray("IFC4.IFCCONIC", "IFC4.IFCBOUNDEDCURVE") * TYPEOF(Directrix)) == 1);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSweptDiskSolid.DirectrixBounded' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcSweptDiskSolid.DirectrixDim))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptDiskSolid.DirectrixDim", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSweptDiskSolid.InnerRadiusSize))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptDiskSolid.InnerRadiusSize", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSweptDiskSolid.DirectrixBounded))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptDiskSolid.DirectrixBounded", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcSweptDiskSolid
	{
		public static readonly IfcSweptDiskSolid DirectrixDim = new IfcSweptDiskSolid();
		public static readonly IfcSweptDiskSolid InnerRadiusSize = new IfcSweptDiskSolid();
		public static readonly IfcSweptDiskSolid DirectrixBounded = new IfcSweptDiskSolid();
		protected IfcSweptDiskSolid() {}
	}
}
