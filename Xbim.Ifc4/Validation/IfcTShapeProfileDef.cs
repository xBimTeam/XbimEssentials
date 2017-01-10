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
namespace Xbim.Ifc4.ProfileResource
{
	public partial class IfcTShapeProfileDef : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTShapeProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcTShapeProfileDef.ValidFlangeThickness) {
				try {
					retVal = FlangeThickness < Depth;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcTShapeProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTShapeProfileDef.ValidFlangeThickness' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcTShapeProfileDef.ValidWebThickness) {
				try {
					retVal = WebThickness < FlangeWidth;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcTShapeProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTShapeProfileDef.ValidWebThickness' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcTShapeProfileDef.ValidFlangeThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTShapeProfileDef.ValidFlangeThickness", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcTShapeProfileDef.ValidWebThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTShapeProfileDef.ValidWebThickness", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcTShapeProfileDef
	{
		public static readonly IfcTShapeProfileDef ValidFlangeThickness = new IfcTShapeProfileDef();
		public static readonly IfcTShapeProfileDef ValidWebThickness = new IfcTShapeProfileDef();
		protected IfcTShapeProfileDef() {}
	}
}
