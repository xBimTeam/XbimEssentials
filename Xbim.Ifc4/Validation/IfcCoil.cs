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
namespace Xbim.Ifc4.HvacDomain
{
	public partial class IfcCoil : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.HvacDomain.IfcCoil");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCoil clause) {
			var retVal = false;
			if (clause == Where.IfcCoil.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcCoilTypeEnum.USERDEFINED) || ((PredefinedType == IfcCoilTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCoil.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCoil.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCCOILTYPE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCoil.CorrectTypeAssigned' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcCoil.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCoil.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCoil.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCoil.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcCoil : IfcProduct
	{
		public static readonly IfcCoil CorrectPredefinedType = new IfcCoil();
		public static readonly IfcCoil CorrectTypeAssigned = new IfcCoil();
		protected IfcCoil() {}
	}
}
