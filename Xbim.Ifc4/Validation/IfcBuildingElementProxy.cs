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
	public partial class IfcBuildingElementProxy : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedBldgElements.IfcBuildingElementProxy");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcBuildingElementProxy clause) {
			var retVal = false;
			if (clause == Where.IfcBuildingElementProxy.HasObjectName) {
				try {
					retVal = EXISTS(this/* as IfcRoot*/.Name);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcBuildingElementProxy.HasObjectName' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcBuildingElementProxy.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcBuildingElementProxyTypeEnum.USERDEFINED) || ((PredefinedType == IfcBuildingElementProxyTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcBuildingElementProxy.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcBuildingElementProxy.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCBUILDINGELEMENTPROXYTYPE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcBuildingElementProxy.CorrectTypeAssigned' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcBuildingElementProxy.HasObjectName))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBuildingElementProxy.HasObjectName", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcBuildingElementProxy.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBuildingElementProxy.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcBuildingElementProxy.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBuildingElementProxy.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcBuildingElementProxy : IfcBuildingElement
	{
		public static readonly IfcBuildingElementProxy HasObjectName = new IfcBuildingElementProxy();
		public static readonly IfcBuildingElementProxy CorrectPredefinedType = new IfcBuildingElementProxy();
		public static readonly IfcBuildingElementProxy CorrectTypeAssigned = new IfcBuildingElementProxy();
		protected IfcBuildingElementProxy() {}
	}
}
