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
namespace Xbim.Ifc4.StructuralAnalysisDomain
{
	public partial class IfcStructuralSurfaceMember : IExpressValidatable
	{
		public enum IfcStructuralSurfaceMemberClause
		{
			HasObjectType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcStructuralSurfaceMemberClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcStructuralSurfaceMemberClause.HasObjectType:
						retVal = (PredefinedType != IfcStructuralSurfaceMemberTypeEnum.USERDEFINED) || Functions.EXISTS(this/* as IfcObject*/.ObjectType);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralSurfaceMember>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcStructuralSurfaceMember.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcStructuralSurfaceMemberClause.HasObjectType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralSurfaceMember.HasObjectType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
