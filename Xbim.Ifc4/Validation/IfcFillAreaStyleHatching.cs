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
namespace Xbim.Ifc4.PresentationAppearanceResource
{
	public partial class IfcFillAreaStyleHatching : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcFillAreaStyleHatching clause) {
			var retVal = false;
			if (clause == Where.IfcFillAreaStyleHatching.PatternStart2D) {
				try {
					retVal = !(EXISTS(PatternStart)) || (PatternStart.Dim == 2);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcFillAreaStyleHatching");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcFillAreaStyleHatching.PatternStart2D' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcFillAreaStyleHatching.RefHatchLine2D) {
				try {
					retVal = !(EXISTS(PointOfReferenceHatchLine)) || (PointOfReferenceHatchLine.Dim == 2);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcFillAreaStyleHatching");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcFillAreaStyleHatching.RefHatchLine2D' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcFillAreaStyleHatching.PatternStart2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyleHatching.PatternStart2D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcFillAreaStyleHatching.RefHatchLine2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyleHatching.RefHatchLine2D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcFillAreaStyleHatching
	{
		public static readonly IfcFillAreaStyleHatching PatternStart2D = new IfcFillAreaStyleHatching();
		public static readonly IfcFillAreaStyleHatching RefHatchLine2D = new IfcFillAreaStyleHatching();
		protected IfcFillAreaStyleHatching() {}
	}
}
