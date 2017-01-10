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
		public enum IfcTShapeProfileDefClause
		{
			ValidFlangeThickness,
			ValidWebThickness,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcTShapeProfileDefClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTShapeProfileDefClause.ValidFlangeThickness:
						retVal = FlangeThickness < Depth;
						break;
					case IfcTShapeProfileDefClause.ValidWebThickness:
						retVal = WebThickness < FlangeWidth;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcTShapeProfileDef");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTShapeProfileDef.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcTShapeProfileDefClause.ValidFlangeThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTShapeProfileDef.ValidFlangeThickness", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcTShapeProfileDefClause.ValidWebThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTShapeProfileDef.ValidWebThickness", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
