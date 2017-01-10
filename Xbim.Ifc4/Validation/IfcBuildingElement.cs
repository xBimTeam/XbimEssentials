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
namespace Xbim.Ifc4.ProductExtension
{
	public partial class IfcBuildingElement : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcBuildingElement clause) {
			var retVal = false;
			if (clause == Where.IfcBuildingElement.MaxOneMaterialAssociation) {
				try {
					retVal = SIZEOF(this/* as IfcObjectDefinition*/.HasAssociations.Where(temp => TYPEOF(temp).Contains("IFC4.IFCRELASSOCIATESMATERIAL"))) <= 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcBuildingElement");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcBuildingElement.MaxOneMaterialAssociation' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcProduct)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcBuildingElement.MaxOneMaterialAssociation))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBuildingElement.MaxOneMaterialAssociation", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcBuildingElement : IfcProduct
	{
		public static readonly IfcBuildingElement MaxOneMaterialAssociation = new IfcBuildingElement();
		protected IfcBuildingElement() {}
	}
}
