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
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRelAssociatesMaterial clause) {
			var retVal = false;
			if (clause == Where.IfcRelAssociatesMaterial.NoVoidElement) {
				try {
					retVal = SIZEOF(this/* as IfcRelAssociates*/.RelatedObjects.Where(temp => (TYPEOF(temp).Contains("IFC4.IFCFEATUREELEMENTSUBTRACTION")) || (TYPEOF(temp).Contains("IFC4.IFCVIRTUALELEMENT")))) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRelAssociatesMaterial.NoVoidElement' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRelAssociatesMaterial.AllowedElements) {
				try {
					retVal = SIZEOF(this/* as IfcRelAssociates*/.RelatedObjects.Where(temp => (SIZEOF(TYPEOF(temp) * NewArray("IFC4.IFCELEMENT", "IFC4.IFCELEMENTTYPE", "IFC4.IFCWINDOWSTYLE", "IFC4.IFCDOORSTYLE", "IFC4.IFCSTRUCTURALMEMBER", "IFC4.IFCPORT")) == 0))) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRelAssociatesMaterial.AllowedElements' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcRelAssociatesMaterial.NoVoidElement))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelAssociatesMaterial.NoVoidElement", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRelAssociatesMaterial.AllowedElements))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelAssociatesMaterial.AllowedElements", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcRelAssociatesMaterial
	{
		public static readonly IfcRelAssociatesMaterial NoVoidElement = new IfcRelAssociatesMaterial();
		public static readonly IfcRelAssociatesMaterial AllowedElements = new IfcRelAssociatesMaterial();
		protected IfcRelAssociatesMaterial() {}
	}
}
