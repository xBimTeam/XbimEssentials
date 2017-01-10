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
	public partial class IfcVoidingFeature : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcVoidingFeature clause) {
			var retVal = false;
			if (clause == Where.IfcVoidingFeature.HasObjectType) {
				try {
					retVal = !EXISTS(PredefinedType) || (PredefinedType != IfcVoidingFeatureTypeEnum.USERDEFINED) || EXISTS(this/* as IfcObject*/.ObjectType);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralElementsDomain.IfcVoidingFeature");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcVoidingFeature.HasObjectType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcFeatureElementSubtraction)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcVoidingFeature.HasObjectType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcVoidingFeature.HasObjectType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcVoidingFeature : IfcFeatureElementSubtraction
	{
		public static readonly IfcVoidingFeature HasObjectType = new IfcVoidingFeature();
		protected IfcVoidingFeature() {}
	}
}
