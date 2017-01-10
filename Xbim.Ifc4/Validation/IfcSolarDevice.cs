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
	public partial class IfcSolarDevice : IExpressValidatable
	{
		public enum IfcSolarDeviceClause
		{
			CorrectPredefinedType,
			CorrectTypeAssigned,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcSolarDeviceClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcSolarDeviceClause.CorrectPredefinedType:
						retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcSolarDeviceTypeEnum.USERDEFINED) || ((PredefinedType == IfcSolarDeviceTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
						break;
					case IfcSolarDeviceClause.CorrectTypeAssigned:
						retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCSOLARDEVICETYPE"));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.ElectricalDomain.IfcSolarDevice");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSolarDevice.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcSolarDeviceClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSolarDevice.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSolarDeviceClause.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSolarDevice.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
