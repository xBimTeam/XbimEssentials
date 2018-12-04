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
namespace Xbim.Ifc2x3.SharedFacilitiesElements
{
	public partial class IfcAsset : IExpressValidatable
	{
		public enum IfcAssetClause
		{
			WR1,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcAssetClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcAssetClause.WR1:
						retVal = Functions.SIZEOF(this/* as IfcGroup*/.IsGroupedBy.RelatedObjects.Where(Temp => !(Functions.TYPEOF(Temp).Contains("IFC2X3.IFCELEMENT")))) == 0;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.SharedFacilitiesElements.IfcAsset>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcAsset.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcAssetClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAsset.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
