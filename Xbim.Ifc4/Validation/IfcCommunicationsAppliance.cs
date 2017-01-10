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
	public partial class IfcCommunicationsAppliance : IExpressValidatable
	{
		public enum IfcCommunicationsApplianceClause
		{
			CorrectPredefinedType,
			CorrectTypeAssigned,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCommunicationsApplianceClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCommunicationsApplianceClause.CorrectPredefinedType:
						retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcCommunicationsApplianceTypeEnum.USERDEFINED) || ((PredefinedType == IfcCommunicationsApplianceTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
						break;
					case IfcCommunicationsApplianceClause.CorrectTypeAssigned:
						retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCCOMMUNICATIONSAPPLIANCETYPE"));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.ElectricalDomain.IfcCommunicationsAppliance");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCommunicationsAppliance.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcCommunicationsApplianceClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCommunicationsAppliance.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcCommunicationsApplianceClause.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCommunicationsAppliance.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
