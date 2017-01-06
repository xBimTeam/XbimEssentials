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
namespace Xbim.Ifc4.StructuralElementsDomain
{
	public partial class IfcReinforcingMesh : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralElementsDomain.IfcReinforcingMesh");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcReinforcingMesh clause) {
			var retVal = false;
			if (clause == Where.IfcReinforcingMesh.CorrectPredefinedType) {
				try {
					retVal = !EXISTS(PredefinedType) || (PredefinedType != IfcReinforcingMeshTypeEnum.USERDEFINED) || ((PredefinedType == IfcReinforcingMeshTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcReinforcingMesh.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcReinforcingMesh.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCREINFORCINGMESHTYPE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcReinforcingMesh.CorrectTypeAssigned' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcReinforcingMesh.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcReinforcingMesh.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcReinforcingMesh.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcReinforcingMesh.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcReinforcingMesh : IfcProduct
	{
		public static readonly IfcReinforcingMesh CorrectPredefinedType = new IfcReinforcingMesh();
		public static readonly IfcReinforcingMesh CorrectTypeAssigned = new IfcReinforcingMesh();
		protected IfcReinforcingMesh() {}
	}
}
