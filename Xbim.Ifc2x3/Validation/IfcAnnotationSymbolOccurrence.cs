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
namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
	public partial class IfcAnnotationSymbolOccurrence : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDefinitionResource.IfcAnnotationSymbolOccurrence");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcAnnotationSymbolOccurrence clause) {
			var retVal = false;
			if (clause == Where.IfcAnnotationSymbolOccurrence.WR31) {
				try {
					retVal = !(EXISTS(this/* as IfcStyledItem*/.Item)) || (TYPEOF(this/* as IfcStyledItem*/.Item).Contains("IFC2X3.IFCDEFINEDSYMBOL"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcAnnotationSymbolOccurrence.WR31' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcStyledItem)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcAnnotationSymbolOccurrence.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAnnotationSymbolOccurrence.WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcAnnotationSymbolOccurrence : IfcStyledItem
	{
		public static readonly IfcAnnotationSymbolOccurrence WR31 = new IfcAnnotationSymbolOccurrence();
		protected IfcAnnotationSymbolOccurrence() {}
	}
}
