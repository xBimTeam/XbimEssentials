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
	public partial class IfcAsymmetricIShapeProfileDef : IExpressValidatable
	{
		public enum IfcAsymmetricIShapeProfileDefClause
		{
			ValidFlangeThickness,
			ValidWebThickness,
			ValidBottomFilletRadius,
			ValidTopFilletRadius,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcAsymmetricIShapeProfileDefClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcAsymmetricIShapeProfileDefClause.ValidFlangeThickness:
						retVal = !(EXISTS(TopFlangeThickness)) || ((BottomFlangeThickness + TopFlangeThickness) < OverallDepth);
						break;
					case IfcAsymmetricIShapeProfileDefClause.ValidWebThickness:
						retVal = (WebThickness < BottomFlangeWidth) && (WebThickness < TopFlangeWidth);
						break;
					case IfcAsymmetricIShapeProfileDefClause.ValidBottomFilletRadius:
						retVal = (!(EXISTS(BottomFlangeFilletRadius))) || (BottomFlangeFilletRadius <= (BottomFlangeWidth - WebThickness) / 2);
						break;
					case IfcAsymmetricIShapeProfileDefClause.ValidTopFilletRadius:
						retVal = (!(EXISTS(TopFlangeFilletRadius))) || (TopFlangeFilletRadius <= (TopFlangeWidth - WebThickness) / 2);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcAsymmetricIShapeProfileDef");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAsymmetricIShapeProfileDef.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcAsymmetricIShapeProfileDefClause.ValidFlangeThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAsymmetricIShapeProfileDef.ValidFlangeThickness", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAsymmetricIShapeProfileDefClause.ValidWebThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAsymmetricIShapeProfileDef.ValidWebThickness", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAsymmetricIShapeProfileDefClause.ValidBottomFilletRadius))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAsymmetricIShapeProfileDef.ValidBottomFilletRadius", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAsymmetricIShapeProfileDefClause.ValidTopFilletRadius))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAsymmetricIShapeProfileDef.ValidTopFilletRadius", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
