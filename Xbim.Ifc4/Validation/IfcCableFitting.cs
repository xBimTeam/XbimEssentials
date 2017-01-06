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
namespace Xbim.Ifc4.ElectricalDomain
{
	public partial class IfcCableFitting : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ElectricalDomain.IfcCableFitting");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCableFitting clause) {
			var retVal = false;
			if (clause == Where.IfcCableFitting.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcCableFittingTypeEnum.USERDEFINED) || ((PredefinedType == IfcCableFittingTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCableFitting.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCableFitting.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCCABLEFITTINGTYPE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCableFitting.CorrectTypeAssigned' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcProduct)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcCableFitting.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCableFitting.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCableFitting.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCableFitting.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcCableFitting : IfcProduct
	{
		public static readonly IfcCableFitting CorrectPredefinedType = new IfcCableFitting();
		public static readonly IfcCableFitting CorrectTypeAssigned = new IfcCableFitting();
		protected IfcCableFitting() {}
	}
}
