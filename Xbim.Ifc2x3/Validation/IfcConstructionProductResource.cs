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
	public partial class IfcConstructionProductResource : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcConstructionProductResource clause) {
			var retVal = false;
			if (clause == Where.IfcConstructionProductResource.WR1) {
				try {
					retVal = SIZEOF(this/* as IfcResource*/.ResourceOf) <= 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ConstructionMgmtDomain.IfcConstructionProductResource");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcConstructionProductResource.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcConstructionProductResource.WR2) {
				try {
					retVal = !(EXISTS(this/* as IfcResource*/.ResourceOf.ItemAt(0))) || (this/* as IfcResource*/.ResourceOf.ItemAt(0).RelatedObjectsType == IfcObjectTypeEnum.PRODUCT);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ConstructionMgmtDomain.IfcConstructionProductResource");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcConstructionProductResource.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcObject)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcConstructionProductResource.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcConstructionProductResource.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcConstructionProductResource.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcConstructionProductResource.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcConstructionProductResource : IfcObject
	{
		public new static readonly IfcConstructionProductResource WR1 = new IfcConstructionProductResource();
		public static readonly IfcConstructionProductResource WR2 = new IfcConstructionProductResource();
		protected IfcConstructionProductResource() {}
	}
}
