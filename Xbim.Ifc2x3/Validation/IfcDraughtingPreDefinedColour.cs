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
	public partial class IfcDraughtingPreDefinedColour : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcDraughtingPreDefinedColour clause) {
			var retVal = false;
			if (clause == Where.IfcDraughtingPreDefinedColour.WR31) {
				try {
					retVal = NewArray("black", "red", "green", "blue", "yellow", "magenta", "cyan", "white", "by layer").Contains(this/* as IfcPreDefinedItem*/.Name);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationResource.IfcDraughtingPreDefinedColour");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDraughtingPreDefinedColour.WR31' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcDraughtingPreDefinedColour.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDraughtingPreDefinedColour.WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcDraughtingPreDefinedColour
	{
		public static readonly IfcDraughtingPreDefinedColour WR31 = new IfcDraughtingPreDefinedColour();
		protected IfcDraughtingPreDefinedColour() {}
	}
}
