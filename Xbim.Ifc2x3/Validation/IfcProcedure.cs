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
	public partial class IfcProcedure : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcProcedure clause) {
			var retVal = false;
			if (clause == Where.IfcProcedure.WR1) {
				try {
					retVal = SIZEOF(this/* as IfcObjectDefinition*/.Decomposes.Where(temp => !(TYPEOF(temp).Contains("IFC2X3.IFCRELNESTS")))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProcessExtension.IfcProcedure");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcProcedure.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcProcedure.WR2) {
				try {
					retVal = SIZEOF(this/* as IfcObjectDefinition*/.IsDecomposedBy.Where(temp => !(TYPEOF(temp).Contains("IFC2X3.IFCRELNESTS")))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProcessExtension.IfcProcedure");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcProcedure.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcProcedure.WR3) {
				try {
					retVal = EXISTS(this/* as IfcRoot*/.Name);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProcessExtension.IfcProcedure");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcProcedure.WR3' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcProcedure.WR4) {
				try {
					retVal = (ProcedureType != IfcProcedureTypeEnum.USERDEFINED) || ((ProcedureType == IfcProcedureTypeEnum.USERDEFINED) && EXISTS(this/* as IfcProcedure*/.UserDefinedProcedureType));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProcessExtension.IfcProcedure");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcProcedure.WR4' for #{0}.",EntityLabel), ex);
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
			if (!ValidateClause(Where.IfcProcedure.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProcedure.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcProcedure.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProcedure.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcProcedure.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProcedure.WR3", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcProcedure.WR4))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProcedure.WR4", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcProcedure : IfcObject
	{
		public new static readonly IfcProcedure WR1 = new IfcProcedure();
		public static readonly IfcProcedure WR2 = new IfcProcedure();
		public static readonly IfcProcedure WR3 = new IfcProcedure();
		public static readonly IfcProcedure WR4 = new IfcProcedure();
		protected IfcProcedure() {}
	}
}
