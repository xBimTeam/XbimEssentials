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
namespace Xbim.Ifc4.SharedBldgElements
{
	public partial class IfcMemberStandardCase : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcMemberStandardCase clause) {
			var retVal = false;
			if (clause == Where.IfcMemberStandardCase.HasMaterialProfileSetUsage) {
				try {
					retVal = SIZEOF(USEDIN(this, "IFC4.IFCRELASSOCIATES.RELATEDOBJECTS").Where(temp => (TYPEOF(temp).Contains("IFC4.IFCRELASSOCIATESMATERIAL")) && (TYPEOF(temp.AsIfcRelAssociatesMaterial().RelatingMaterial).Contains("IFC4.IFCMATERIALPROFILESETUSAGE")))) == 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedBldgElements.IfcMemberStandardCase");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcMemberStandardCase.HasMaterialProfileSetUsage' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcMember)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcMemberStandardCase.HasMaterialProfileSetUsage))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMemberStandardCase.HasMaterialProfileSetUsage", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcMemberStandardCase : IfcMember
	{
		public static readonly IfcMemberStandardCase HasMaterialProfileSetUsage = new IfcMemberStandardCase();
		protected IfcMemberStandardCase() {}
	}
}
