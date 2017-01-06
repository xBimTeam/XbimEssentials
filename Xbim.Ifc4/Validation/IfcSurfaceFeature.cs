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
	public partial class IfcSurfaceFeature : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralElementsDomain.IfcSurfaceFeature");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSurfaceFeature clause) {
			var retVal = false;
			if (clause == Where.IfcSurfaceFeature.HasObjectType) {
				try {
					retVal = !EXISTS(PredefinedType) || (PredefinedType != IfcSurfaceFeatureTypeEnum.USERDEFINED) || EXISTS(this/* as IfcObject*/.ObjectType);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSurfaceFeature.HasObjectType' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcSurfaceFeature.HasObjectType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceFeature.HasObjectType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcSurfaceFeature : IfcProduct
	{
		public static readonly IfcSurfaceFeature HasObjectType = new IfcSurfaceFeature();
		protected IfcSurfaceFeature() {}
	}
}
