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
namespace Xbim.Ifc4.ConstructionMgmtDomain
{
	public partial class IfcSubContractResourceType : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSubContractResourceType clause) {
			var retVal = false;
			if (clause == Where.IfcSubContractResourceType.CorrectPredefinedType) {
				try {
					retVal = (PredefinedType != IfcSubContractResourceTypeEnum.USERDEFINED) || ((PredefinedType == IfcSubContractResourceTypeEnum.USERDEFINED) && EXISTS(this/* as IfcTypeResource*/.ResourceType));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ConstructionMgmtDomain.IfcSubContractResourceType");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSubContractResourceType.CorrectPredefinedType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcTypeObject)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcSubContractResourceType.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSubContractResourceType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcSubContractResourceType : IfcTypeObject
	{
		public static readonly IfcSubContractResourceType CorrectPredefinedType = new IfcSubContractResourceType();
		protected IfcSubContractResourceType() {}
	}
}
