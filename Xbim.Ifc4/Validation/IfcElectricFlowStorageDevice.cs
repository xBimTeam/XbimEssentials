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
	public partial class IfcElectricFlowStorageDevice : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ElectricalDomain.IfcElectricFlowStorageDevice");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcElectricFlowStorageDevice clause) {
			var retVal = false;
			if (clause == Where.IfcElectricFlowStorageDevice.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcElectricFlowStorageDeviceTypeEnum.USERDEFINED) || ((PredefinedType == IfcElectricFlowStorageDeviceTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcElectricFlowStorageDevice.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcElectricFlowStorageDevice.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCELECTRICFLOWSTORAGEDEVICETYPE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcElectricFlowStorageDevice.CorrectTypeAssigned' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcElectricFlowStorageDevice.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcElectricFlowStorageDevice.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcElectricFlowStorageDevice.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcElectricFlowStorageDevice.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcElectricFlowStorageDevice : IfcProduct
	{
		public static readonly IfcElectricFlowStorageDevice CorrectPredefinedType = new IfcElectricFlowStorageDevice();
		public static readonly IfcElectricFlowStorageDevice CorrectTypeAssigned = new IfcElectricFlowStorageDevice();
		protected IfcElectricFlowStorageDevice() {}
	}
}
