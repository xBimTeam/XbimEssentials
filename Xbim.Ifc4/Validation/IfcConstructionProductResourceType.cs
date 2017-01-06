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
	public partial class IfcConstructionProductResourceType : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ConstructionMgmtDomain.IfcConstructionProductResourceType");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcConstructionProductResourceType clause) {
			var retVal = false;
			if (clause == Where.IfcConstructionProductResourceType.CorrectPredefinedType) {
				try {
					retVal = (PredefinedType != IfcConstructionProductResourceTypeEnum.USERDEFINED) || ((PredefinedType == IfcConstructionProductResourceTypeEnum.USERDEFINED) && EXISTS(this/* as IfcTypeResource*/.ResourceType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcConstructionProductResourceType.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcTypeObject)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcConstructionProductResourceType.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcConstructionProductResourceType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcConstructionProductResourceType : IfcTypeObject
	{
		public static readonly IfcConstructionProductResourceType CorrectPredefinedType = new IfcConstructionProductResourceType();
		protected IfcConstructionProductResourceType() {}
	}
}
