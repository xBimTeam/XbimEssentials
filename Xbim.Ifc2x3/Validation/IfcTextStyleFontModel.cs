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
	public partial class IfcTextStyleFontModel : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTextStyleFontModel clause) {
			var retVal = false;
			if (clause == Where.IfcTextStyleFontModel.WR31) {
				try {
					retVal = (TYPEOF(this.FontSize).Contains("IFC2X3.IFCLENGTHMEASURE")) && (this.FontSize.AsIfcLengthMeasure() > 0);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationResource.IfcTextStyleFontModel");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTextStyleFontModel.WR31' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcTextStyleFontModel.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTextStyleFontModel.WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcTextStyleFontModel
	{
		public static readonly IfcTextStyleFontModel WR31 = new IfcTextStyleFontModel();
		protected IfcTextStyleFontModel() {}
	}
}
