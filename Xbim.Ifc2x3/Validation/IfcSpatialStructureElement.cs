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
		public enum IfcSpatialStructureElementClause
		{
			WR41,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcSpatialStructureElementClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcSpatialStructureElementClause.WR41:
						retVal = (HIINDEX(this/* as IfcObjectDefinition*/.Decomposes) == 1) && (TYPEOF(this/* as IfcObjectDefinition*/.Decomposes.ItemAt(0)).Contains("IFC2X3.IFCRELAGGREGATES")) && ((TYPEOF(this/* as IfcObjectDefinition*/.Decomposes.ItemAt(0).RelatingObject).Contains("IFC2X3.IFCPROJECT")) || (TYPEOF(this/* as IfcObjectDefinition*/.Decomposes.ItemAt(0).RelatingObject).Contains("IFC2X3.IFCSPATIALSTRUCTUREELEMENT")));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc2x3.ProductExtension.IfcSpatialStructureElement");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSpatialStructureElement.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcSpatialStructureElementClause.WR41))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSpatialStructureElement.WR41", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
