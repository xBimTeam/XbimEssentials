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
namespace Xbim.Ifc4.SharedBldgServiceElements
{
	public partial class IfcDistributionChamberElement : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedBldgServiceElements.IfcDistributionChamberElement");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcDistributionChamberElement clause) {
			var retVal = false;
			if (clause == Where.IfcDistributionChamberElement.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcDistributionChamberElementTypeEnum.USERDEFINED) || ((PredefinedType == IfcDistributionChamberElementTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcDistributionChamberElement.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcDistributionChamberElement.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCDISTRIBUTIONCHAMBERELEMENTTYPE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcDistributionChamberElement.CorrectTypeAssigned' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcDistributionChamberElement.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDistributionChamberElement.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcDistributionChamberElement.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDistributionChamberElement.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcDistributionChamberElement : IfcProduct
	{
		public static readonly IfcDistributionChamberElement CorrectPredefinedType = new IfcDistributionChamberElement();
		public static readonly IfcDistributionChamberElement CorrectTypeAssigned = new IfcDistributionChamberElement();
		protected IfcDistributionChamberElement() {}
	}
}
