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
namespace Xbim.Ifc2x3.ProductExtension
{
	public partial class IfcRelAssociatesMaterial : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProductExtension.IfcRelAssociatesMaterial");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRelAssociatesMaterial clause) {
			var retVal = false;
			if (clause == Where.IfcRelAssociatesMaterial.WR21) {
				try {
					retVal = SIZEOF(this/* as IfcRelAssociates*/.RelatedObjects.Where(temp => (TYPEOF(temp).Contains("IFC2X3.IFCFEATUREELEMENTSUBTRACTION")) || (TYPEOF(temp).Contains("IFC2X3.IFCVIRTUALELEMENT")))) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRelAssociatesMaterial.WR21' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRelAssociatesMaterial.WR22) {
				try {
					retVal = SIZEOF(this/* as IfcRelAssociates*/.RelatedObjects.Where(temp => (!(TYPEOF(temp).Contains("IFC2X3.IFCPRODUCT")) && !(TYPEOF(temp).Contains("IFC2X3.IFCTYPEPRODUCT"))))) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRelAssociatesMaterial.WR22' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcRelAssociates)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcRelAssociatesMaterial.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelAssociatesMaterial.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRelAssociatesMaterial.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelAssociatesMaterial.WR22", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcRelAssociatesMaterial : IfcRelAssociates
	{
		public new static readonly IfcRelAssociatesMaterial WR21 = new IfcRelAssociatesMaterial();
		public static readonly IfcRelAssociatesMaterial WR22 = new IfcRelAssociatesMaterial();
		protected IfcRelAssociatesMaterial() {}
	}
}
