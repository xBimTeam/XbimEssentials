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
namespace Xbim.Ifc2x3.ProcessExtension
{
	public partial class IfcTask : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTask clause) {
			var retVal = false;
			if (clause == Where.IfcTask.WR1) {
				try {
					retVal = SIZEOF(this/* as IfcObjectDefinition*/.Decomposes.Where(temp => !(TYPEOF(temp).Contains("IFC2X3.IFCRELNESTS")))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProcessExtension.IfcTask");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTask.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcTask.WR2) {
				try {
					retVal = SIZEOF(this/* as IfcObjectDefinition*/.IsDecomposedBy.Where(temp => !(TYPEOF(temp).Contains("IFC2X3.IFCRELNESTS")))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProcessExtension.IfcTask");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTask.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcTask.WR3) {
				try {
					retVal = EXISTS(this/* as IfcRoot*/.Name);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProcessExtension.IfcTask");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTask.WR3' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcObject)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcTask.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTask.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcTask.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTask.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcTask.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTask.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcTask : IfcObject
	{
		public new static readonly IfcTask WR1 = new IfcTask();
		public static readonly IfcTask WR2 = new IfcTask();
		public static readonly IfcTask WR3 = new IfcTask();
		protected IfcTask() {}
	}
}
