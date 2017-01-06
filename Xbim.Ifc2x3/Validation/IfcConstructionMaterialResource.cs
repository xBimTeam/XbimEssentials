using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
using static Xbim.Ifc2x3.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.ConstructionMgmtDomain
{
	public partial class IfcConstructionMaterialResource : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ConstructionMgmtDomain.IfcConstructionMaterialResource");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcConstructionMaterialResource clause) {
			var retVal = false;
			if (clause == Where.IfcConstructionMaterialResource.WR1) {
				try {
					retVal = SIZEOF(this/* as IfcResource*/.ResourceOf) <= 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcConstructionMaterialResource.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcConstructionMaterialResource.WR2) {
				try {
					retVal = !(EXISTS(this/* as IfcResource*/.ResourceOf.ToArray()[0])) || (this/* as IfcResource*/.ResourceOf.ToArray()[0].RelatedObjectsType == IfcObjectTypeEnum.PRODUCT);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcConstructionMaterialResource.WR2' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcObject)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcConstructionMaterialResource.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcConstructionMaterialResource.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcConstructionMaterialResource.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcConstructionMaterialResource.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcConstructionMaterialResource : IfcObject
	{
		public new static readonly IfcConstructionMaterialResource WR1 = new IfcConstructionMaterialResource();
		public static readonly IfcConstructionMaterialResource WR2 = new IfcConstructionMaterialResource();
		protected IfcConstructionMaterialResource() {}
	}
}
