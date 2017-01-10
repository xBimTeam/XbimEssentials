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
namespace Xbim.Ifc2x3.SharedFacilitiesElements
{
	public partial class IfcInventory : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcInventory clause) {
			var retVal = false;
			if (clause == Where.IfcInventory.WR41) {
				try {
					retVal = SIZEOF(this/* as IfcGroup*/.IsGroupedBy.RelatedObjects.Where(temp => !((TYPEOF(temp).Contains("IFC2X3.IFCSPACE")) || (TYPEOF(temp).Contains("IFC2X3.IFCASSET")) || (TYPEOF(temp).Contains("IFC2X3.IFCFURNISHINGELEMENT"))))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.SharedFacilitiesElements.IfcInventory");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcInventory.WR41' for #{0}.",EntityLabel), ex);
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
			if (!ValidateClause(Where.IfcInventory.WR41))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcInventory.WR41", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcInventory : IfcObject
	{
		public static readonly IfcInventory WR41 = new IfcInventory();
		protected IfcInventory() {}
	}
}
