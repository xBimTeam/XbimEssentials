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
	public partial class IfcRelSpaceBoundary : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProductExtension.IfcRelSpaceBoundary");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRelSpaceBoundary clause) {
			var retVal = false;
			if (clause == Where.IfcRelSpaceBoundary.WR1) {
				try {
					retVal = ((PhysicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.PHYSICAL) && (EXISTS(RelatedBuildingElement) && !(TYPEOF(RelatedBuildingElement).Contains("IFC2X3.IFCVIRTUALELEMENT")))) || ((PhysicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.VIRTUAL) && (!(EXISTS(RelatedBuildingElement)) || (TYPEOF(RelatedBuildingElement).Contains("IFC2X3.IFCVIRTUALELEMENT")))) || (PhysicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.NOTDEFINED);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRelSpaceBoundary.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcRelSpaceBoundary.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelSpaceBoundary.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcRelSpaceBoundary
	{
		public static readonly IfcRelSpaceBoundary WR1 = new IfcRelSpaceBoundary();
		protected IfcRelSpaceBoundary() {}
	}
}
