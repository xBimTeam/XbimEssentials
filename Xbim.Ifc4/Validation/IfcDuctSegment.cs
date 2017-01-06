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
	public partial class IfcDuctSegment : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.HvacDomain.IfcDuctSegment");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcDuctSegment clause) {
			var retVal = false;
			if (clause == Where.IfcDuctSegment.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcDuctSegmentTypeEnum.USERDEFINED) || ((PredefinedType == IfcDuctSegmentTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcDuctSegment.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcDuctSegment.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCDUCTSEGMENTTYPE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcDuctSegment.CorrectTypeAssigned' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcDuctSegment.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDuctSegment.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcDuctSegment.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDuctSegment.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcDuctSegment : IfcProduct
	{
		public static readonly IfcDuctSegment CorrectPredefinedType = new IfcDuctSegment();
		public static readonly IfcDuctSegment CorrectTypeAssigned = new IfcDuctSegment();
		protected IfcDuctSegment() {}
	}
}
