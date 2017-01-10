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
namespace Xbim.Ifc2x3.GeometricModelResource
{
	public partial class IfcSectionedSpine : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSectionedSpine clause) {
			var retVal = false;
			if (clause == Where.IfcSectionedSpine.WR1) {
				try {
					retVal = SIZEOF(CrossSections) == SIZEOF(CrossSectionPositions);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometricModelResource.IfcSectionedSpine");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSectionedSpine.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSectionedSpine.WR2) {
				try {
					retVal = SIZEOF(CrossSections.Where(temp => CrossSections.ItemAt(0).ProfileType != temp.ProfileType)) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometricModelResource.IfcSectionedSpine");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSectionedSpine.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSectionedSpine.WR3) {
				try {
					retVal = SpineCurve.Dim == 3;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometricModelResource.IfcSectionedSpine");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSectionedSpine.WR3' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcSectionedSpine.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSectionedSpine.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSectionedSpine.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSectionedSpine.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSectionedSpine.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSectionedSpine.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcSectionedSpine
	{
		public static readonly IfcSectionedSpine WR1 = new IfcSectionedSpine();
		public static readonly IfcSectionedSpine WR2 = new IfcSectionedSpine();
		public static readonly IfcSectionedSpine WR3 = new IfcSectionedSpine();
		protected IfcSectionedSpine() {}
	}
}
