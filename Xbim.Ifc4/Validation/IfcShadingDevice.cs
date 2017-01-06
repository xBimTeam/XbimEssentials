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
namespace Xbim.Ifc4.SharedBldgElements
{
	public partial class IfcShadingDevice : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedBldgElements.IfcShadingDevice");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcShadingDevice clause) {
			var retVal = false;
			if (clause == Where.IfcShadingDevice.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcShadingDeviceTypeEnum.USERDEFINED) || ((PredefinedType == IfcShadingDeviceTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcShadingDevice.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcShadingDevice.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCSHADINGDEVICETYPE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcShadingDevice.CorrectTypeAssigned' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcBuildingElement)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcShadingDevice.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShadingDevice.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcShadingDevice.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcShadingDevice.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcShadingDevice : IfcBuildingElement
	{
		public static readonly IfcShadingDevice CorrectPredefinedType = new IfcShadingDevice();
		public static readonly IfcShadingDevice CorrectTypeAssigned = new IfcShadingDevice();
		protected IfcShadingDevice() {}
	}
}
