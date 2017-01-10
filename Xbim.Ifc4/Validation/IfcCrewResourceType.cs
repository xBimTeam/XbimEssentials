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
	public partial class IfcCrewResourceType : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCrewResourceType clause) {
			var retVal = false;
			if (clause == Where.IfcCrewResourceType.CorrectPredefinedType) {
				try {
					retVal = (PredefinedType != IfcCrewResourceTypeEnum.USERDEFINED) || ((PredefinedType == IfcCrewResourceTypeEnum.USERDEFINED) && EXISTS(this/* as IfcTypeResource*/.ResourceType));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ConstructionMgmtDomain.IfcCrewResourceType");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCrewResourceType.CorrectPredefinedType' for #{0}.",EntityLabel), ex);
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
			if (!ValidateClause(Where.IfcCrewResourceType.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCrewResourceType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcCrewResourceType : IfcTypeObject
	{
		public static readonly IfcCrewResourceType CorrectPredefinedType = new IfcCrewResourceType();
		protected IfcCrewResourceType() {}
	}
}
