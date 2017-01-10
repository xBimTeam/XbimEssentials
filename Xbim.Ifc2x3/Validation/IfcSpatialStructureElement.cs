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
namespace Xbim.Ifc2x3.ProductExtension
{
	public partial class IfcSpatialStructureElement : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSpatialStructureElement clause) {
			var retVal = false;
			if (clause == Where.IfcSpatialStructureElement.WR41) {
				try {
					retVal = (HIINDEX(this/* as IfcObjectDefinition*/.Decomposes) == 1) && (TYPEOF(this/* as IfcObjectDefinition*/.Decomposes.ItemAt(0)).Contains("IFC2X3.IFCRELAGGREGATES")) && ((TYPEOF(this/* as IfcObjectDefinition*/.Decomposes.ItemAt(0).RelatingObject).Contains("IFC2X3.IFCPROJECT")) || (TYPEOF(this/* as IfcObjectDefinition*/.Decomposes.ItemAt(0).RelatingObject).Contains("IFC2X3.IFCSPATIALSTRUCTUREELEMENT")));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProductExtension.IfcSpatialStructureElement");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSpatialStructureElement.WR41' for #{0}.",EntityLabel), ex);
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
			if (!ValidateClause(Where.IfcSpatialStructureElement.WR41))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSpatialStructureElement.WR41", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcSpatialStructureElement : IfcProduct
	{
		public static readonly IfcSpatialStructureElement WR41 = new IfcSpatialStructureElement();
		protected IfcSpatialStructureElement() {}
	}
}
