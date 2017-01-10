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
	public partial class IfcTextLiteralWithExtent : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTextLiteralWithExtent clause) {
			var retVal = false;
			if (clause == Where.IfcTextLiteralWithExtent.WR31) {
				try {
					retVal = !(TYPEOF(Extent).Contains("IFC2X3.IFCPLANARBOX"));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDefinitionResource.IfcTextLiteralWithExtent");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTextLiteralWithExtent.WR31' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcTextLiteralWithExtent.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTextLiteralWithExtent.WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcTextLiteralWithExtent
	{
		public static readonly IfcTextLiteralWithExtent WR31 = new IfcTextLiteralWithExtent();
		protected IfcTextLiteralWithExtent() {}
	}
}
