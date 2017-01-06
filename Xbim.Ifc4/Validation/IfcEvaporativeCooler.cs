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
	public partial class IfcEvaporativeCooler : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.HvacDomain.IfcEvaporativeCooler");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcEvaporativeCooler clause) {
			var retVal = false;
			if (clause == Where.IfcEvaporativeCooler.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcEvaporativeCoolerTypeEnum.USERDEFINED) || ((PredefinedType == IfcEvaporativeCoolerTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcEvaporativeCooler.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcEvaporativeCooler.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCEVAPORATIVECOOLERTYPE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcEvaporativeCooler.CorrectTypeAssigned' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcEvaporativeCooler.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEvaporativeCooler.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcEvaporativeCooler.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEvaporativeCooler.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcEvaporativeCooler : IfcProduct
	{
		public static readonly IfcEvaporativeCooler CorrectPredefinedType = new IfcEvaporativeCooler();
		public static readonly IfcEvaporativeCooler CorrectTypeAssigned = new IfcEvaporativeCooler();
		protected IfcEvaporativeCooler() {}
	}
}
