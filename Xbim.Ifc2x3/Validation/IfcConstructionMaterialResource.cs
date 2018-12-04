using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
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
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.ConstructionMgmtDomain
{
	public partial class IfcConstructionMaterialResource : IExpressValidatable
	{
		public enum IfcConstructionMaterialResourceClause
		{
			WR1,
			WR2,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcConstructionMaterialResourceClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcConstructionMaterialResourceClause.WR1:
						retVal = Functions.SIZEOF(this/* as IfcResource*/.ResourceOf) <= 1;
						break;
					case IfcConstructionMaterialResourceClause.WR2:
						retVal = !(Functions.EXISTS(this/* as IfcResource*/.ResourceOf.ItemAt(0))) || (this/* as IfcResource*/.ResourceOf.ItemAt(0).RelatedObjectsType == IfcObjectTypeEnum.PRODUCT);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.ConstructionMgmtDomain.IfcConstructionMaterialResource>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcConstructionMaterialResource.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcConstructionMaterialResourceClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcConstructionMaterialResource.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcConstructionMaterialResourceClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcConstructionMaterialResource.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
