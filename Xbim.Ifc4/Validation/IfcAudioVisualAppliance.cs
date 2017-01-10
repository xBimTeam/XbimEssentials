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
	public partial class IfcAudioVisualAppliance : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcAudioVisualAppliance clause) {
			var retVal = false;
			if (clause == Where.IfcAudioVisualAppliance.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcAudioVisualApplianceTypeEnum.USERDEFINED) || ((PredefinedType == IfcAudioVisualApplianceTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ElectricalDomain.IfcAudioVisualAppliance");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAudioVisualAppliance.CorrectPredefinedType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAudioVisualAppliance.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCAUDIOVISUALAPPLIANCETYPE"));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ElectricalDomain.IfcAudioVisualAppliance");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAudioVisualAppliance.CorrectTypeAssigned' for #{0}.",EntityLabel), ex);
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
			if (!ValidateClause(Where.IfcAudioVisualAppliance.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAudioVisualAppliance.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAudioVisualAppliance.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAudioVisualAppliance.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcAudioVisualAppliance : IfcProduct
	{
		public static readonly IfcAudioVisualAppliance CorrectPredefinedType = new IfcAudioVisualAppliance();
		public static readonly IfcAudioVisualAppliance CorrectTypeAssigned = new IfcAudioVisualAppliance();
		protected IfcAudioVisualAppliance() {}
	}
}
