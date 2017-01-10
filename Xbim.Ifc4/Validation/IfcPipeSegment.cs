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
	public partial class IfcPipeSegment : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPipeSegment clause) {
			var retVal = false;
			if (clause == Where.IfcPipeSegment.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcPipeSegmentTypeEnum.USERDEFINED) || ((PredefinedType == IfcPipeSegmentTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.HvacDomain.IfcPipeSegment");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPipeSegment.CorrectPredefinedType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPipeSegment.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCPIPESEGMENTTYPE"));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.HvacDomain.IfcPipeSegment");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPipeSegment.CorrectTypeAssigned' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcProduct)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcPipeSegment.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPipeSegment.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPipeSegment.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPipeSegment.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPipeSegment : IfcProduct
	{
		public static readonly IfcPipeSegment CorrectPredefinedType = new IfcPipeSegment();
		public static readonly IfcPipeSegment CorrectTypeAssigned = new IfcPipeSegment();
		protected IfcPipeSegment() {}
	}
}
