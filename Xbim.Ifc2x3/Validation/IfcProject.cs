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
namespace Xbim.Ifc2x3.Kernel
{
	public partial class IfcProject : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.Kernel.IfcProject");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcProject clause) {
			var retVal = false;
			if (clause == Where.IfcProject.WR31) {
				try {
					retVal = EXISTS(this/* as IfcRoot*/.Name);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcProject.WR31' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcProject.WR32) {
				try {
					retVal = SIZEOF(RepresentationContexts.Where(Temp => TYPEOF(Temp).Contains("IFC2X3.IFCGEOMETRICREPRESENTATIONSUBCONTEXT"))) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcProject.WR32' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcProject.WR33) {
				try {
					retVal = SIZEOF(this/* as IfcObjectDefinition*/.Decomposes) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcProject.WR33' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcObject)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcProject.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProject.WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcProject.WR32))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProject.WR32", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcProject.WR33))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProject.WR33", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcProject : IfcObject
	{
		public static readonly IfcProject WR31 = new IfcProject();
		public static readonly IfcProject WR32 = new IfcProject();
		public static readonly IfcProject WR33 = new IfcProject();
		protected IfcProject() {}
	}
}
