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

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcAsymmetricIShapeProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcAsymmetricIShapeProfileDef.ValidFlangeThickness) {
				try {
					retVal = !(EXISTS(TopFlangeThickness)) || ((BottomFlangeThickness + TopFlangeThickness) < OverallDepth);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcAsymmetricIShapeProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAsymmetricIShapeProfileDef.ValidFlangeThickness' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAsymmetricIShapeProfileDef.ValidWebThickness) {
				try {
					retVal = (WebThickness < BottomFlangeWidth) && (WebThickness < TopFlangeWidth);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcAsymmetricIShapeProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAsymmetricIShapeProfileDef.ValidWebThickness' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAsymmetricIShapeProfileDef.ValidBottomFilletRadius) {
				try {
					retVal = (!(EXISTS(BottomFlangeFilletRadius))) || (BottomFlangeFilletRadius <= (BottomFlangeWidth - WebThickness) / 2);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcAsymmetricIShapeProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAsymmetricIShapeProfileDef.ValidBottomFilletRadius' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAsymmetricIShapeProfileDef.ValidTopFilletRadius) {
				try {
					retVal = (!(EXISTS(TopFlangeFilletRadius))) || (TopFlangeFilletRadius <= (TopFlangeWidth - WebThickness) / 2);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcAsymmetricIShapeProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAsymmetricIShapeProfileDef.ValidTopFilletRadius' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcAsymmetricIShapeProfileDef.ValidFlangeThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAsymmetricIShapeProfileDef.ValidFlangeThickness", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAsymmetricIShapeProfileDef.ValidWebThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAsymmetricIShapeProfileDef.ValidWebThickness", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAsymmetricIShapeProfileDef.ValidBottomFilletRadius))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAsymmetricIShapeProfileDef.ValidBottomFilletRadius", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAsymmetricIShapeProfileDef.ValidTopFilletRadius))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAsymmetricIShapeProfileDef.ValidTopFilletRadius", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcAsymmetricIShapeProfileDef
	{
		public static readonly IfcAsymmetricIShapeProfileDef ValidFlangeThickness = new IfcAsymmetricIShapeProfileDef();
		public static readonly IfcAsymmetricIShapeProfileDef ValidWebThickness = new IfcAsymmetricIShapeProfileDef();
		public static readonly IfcAsymmetricIShapeProfileDef ValidBottomFilletRadius = new IfcAsymmetricIShapeProfileDef();
		public static readonly IfcAsymmetricIShapeProfileDef ValidTopFilletRadius = new IfcAsymmetricIShapeProfileDef();
		protected IfcAsymmetricIShapeProfileDef() {}
	}
}
