using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.SharedBldgElements
{
	public partial class IfcBuildingElementProxy : IExpressValidatable
	{
		public enum IfcBuildingElementProxyClause
		{
			HasObjectName,
			CorrectPredefinedType,
			CorrectTypeAssigned,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcBuildingElementProxyClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcBuildingElementProxyClause.HasObjectName:
						retVal = Functions.EXISTS(this/* as IfcRoot*/.Name);
						break;
					case IfcBuildingElementProxyClause.CorrectPredefinedType:
						retVal = !(Functions.EXISTS(PredefinedType)) || (PredefinedType != IfcBuildingElementProxyTypeEnum.USERDEFINED) || ((PredefinedType == IfcBuildingElementProxyTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcObject*/.ObjectType));
						break;
					case IfcBuildingElementProxyClause.CorrectTypeAssigned:
						retVal = (Functions.SIZEOF(IsTypedBy) == 0) || (Functions.TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCBUILDINGELEMENTPROXYTYPE"));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.SharedBldgElements.IfcBuildingElementProxy>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcBuildingElementProxy.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcBuildingElementProxyClause.HasObjectName))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBuildingElementProxy.HasObjectName", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcBuildingElementProxyClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBuildingElementProxy.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcBuildingElementProxyClause.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBuildingElementProxy.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
