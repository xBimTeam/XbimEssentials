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
namespace Xbim.Ifc2x3.FacilitiesMgmtDomain
{
	public partial class IfcMove : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcMove clause) {
			var retVal = false;
			if (clause == Where.IfcMove.WR1) {
				try {
					retVal = SIZEOF(this/* as IfcProcess*/.OperatesOn) >= 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.FacilitiesMgmtDomain.IfcMove");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcMove.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcMove.WR2) {
				try {
					retVal = SIZEOF(OperatesOn.Where(temp => SIZEOF(temp.RelatedObjects.Where(temp2 => (TYPEOF(temp2).Contains("IFC2X3.IFCACTOR")) || (TYPEOF(temp2).Contains("IFC2X3.IFCEQUIPMENTELEMENT")) || (TYPEOF(temp2).Contains("IFC2X3.IFCFURNISHINGELEMENT")))) >= 1)) >= 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.FacilitiesMgmtDomain.IfcMove");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcMove.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcMove.WR3) {
				try {
					retVal = EXISTS(this/* as IfcRoot*/.Name);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.FacilitiesMgmtDomain.IfcMove");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcMove.WR3' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcTask)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcMove.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMove.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcMove.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMove.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcMove.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMove.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcMove : IfcTask
	{
		public new static readonly IfcMove WR1 = new IfcMove();
		public new static readonly IfcMove WR2 = new IfcMove();
		public new static readonly IfcMove WR3 = new IfcMove();
		protected IfcMove() {}
	}
}
