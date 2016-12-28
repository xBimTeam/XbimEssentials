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
	public partial class IfcRelAssociatesMaterial : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcRelAssociatesMaterial");

		/// <summary>
		/// Tests the express where clause NoVoidElement
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NoVoidElement() {
			var retVal = false;
			try {
				retVal = SIZEOF(this/* as IfcRelAssociates*/.RelatedObjects.Where(temp => (TYPEOF(temp).Contains("IFC4.IFCFEATUREELEMENTSUBTRACTION")) || (TYPEOF(temp).Contains("IFC4.IFCVIRTUALELEMENT")))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NoVoidElement' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause AllowedElements
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool AllowedElements() {
			var retVal = false;
			try {
				retVal = SIZEOF(this/* as IfcRelAssociates*/.RelatedObjects.Where(temp => (SIZEOF(TYPEOF(temp) * NewArray("IFC4.IFCELEMENT", "IFC4.IFCELEMENTTYPE", "IFC4.IFCWINDOWSTYLE", "IFC4.IFCDOORSTYLE", "IFC4.IFCSTRUCTURALMEMBER", "IFC4.IFCPORT")) == 0))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'AllowedElements' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!NoVoidElement())
				yield return new ValidationResult() { Item = this, IssueSource = "NoVoidElement", IssueType = ValidationFlags.EntityWhereClauses };
			if (!AllowedElements())
				yield return new ValidationResult() { Item = this, IssueSource = "AllowedElements", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
