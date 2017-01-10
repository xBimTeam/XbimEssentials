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
	public partial class IfcCableCarrierSegment : IExpressValidatable
	{
		public enum IfcCableCarrierSegmentClause
		{
			CorrectPredefinedType,
			CorrectTypeAssigned,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCableCarrierSegmentClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCableCarrierSegmentClause.CorrectPredefinedType:
						retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcCableCarrierSegmentTypeEnum.USERDEFINED) || ((PredefinedType == IfcCableCarrierSegmentTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
						break;
					case IfcCableCarrierSegmentClause.CorrectTypeAssigned:
						retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCCABLECARRIERSEGMENTTYPE"));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.ElectricalDomain.IfcCableCarrierSegment");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCableCarrierSegment.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcCableCarrierSegmentClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCableCarrierSegment.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcCableCarrierSegmentClause.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCableCarrierSegment.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
