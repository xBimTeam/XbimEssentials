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
namespace Xbim.Ifc2x3.PresentationResource
{
	public partial class IfcDraughtingPreDefinedTextFont : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationResource.IfcDraughtingPreDefinedTextFont");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcDraughtingPreDefinedTextFont clause) {
			var retVal = false;
			if (clause == Where.IfcDraughtingPreDefinedTextFont.WR31) {
				try {
					retVal = NewArray("ISO 3098-1 font A", "ISO 3098-1 font B").Contains(this/* as IfcPreDefinedItem*/.Name);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcDraughtingPreDefinedTextFont.WR31' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcDraughtingPreDefinedTextFont.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDraughtingPreDefinedTextFont.WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcDraughtingPreDefinedTextFont
	{
		public static readonly IfcDraughtingPreDefinedTextFont WR31 = new IfcDraughtingPreDefinedTextFont();
		protected IfcDraughtingPreDefinedTextFont() {}
	}
}
