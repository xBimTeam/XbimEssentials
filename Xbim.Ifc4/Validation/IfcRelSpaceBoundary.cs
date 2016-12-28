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
	public partial class IfcRelSpaceBoundary : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcRelSpaceBoundary");

		/// <summary>
		/// Tests the express where clause CorrectPhysOrVirt
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrectPhysOrVirt() {
			var retVal = false;
			try {
				retVal = ((PhysicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.PHYSICAL) && (!(TYPEOF(RelatedBuildingElement).Contains("IFC4.IFCVIRTUALELEMENT")))) || ((PhysicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.VIRTUAL) && ((TYPEOF(RelatedBuildingElement).Contains("IFC4.IFCVIRTUALELEMENT")) || (TYPEOF(RelatedBuildingElement).Contains("IFC4.IFCOPENINGELEMENT")))) || (PhysicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.NOTDEFINED);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrectPhysOrVirt' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!CorrectPhysOrVirt())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrectPhysOrVirt", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
