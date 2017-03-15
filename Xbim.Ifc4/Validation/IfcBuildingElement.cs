using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.ProductExtension
{
	public partial class IfcBuildingElement : IExpressValidatable
	{
		public enum IfcBuildingElementClause
		{
			MaxOneMaterialAssociation,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcBuildingElementClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcBuildingElementClause.MaxOneMaterialAssociation:
						retVal = Functions.SIZEOF(this/* as IfcObjectDefinition*/.HasAssociations.Where(temp => Functions.TYPEOF(temp).Contains("IFC4.IFCRELASSOCIATESMATERIAL"))) <= 1;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcBuildingElement");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcBuildingElement.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcBuildingElementClause.MaxOneMaterialAssociation))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBuildingElement.MaxOneMaterialAssociation", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
