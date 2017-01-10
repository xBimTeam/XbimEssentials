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
namespace Xbim.Ifc4.GeometryResource
{
	public partial class IfcAxis2Placement2D : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcAxis2Placement2D clause) {
			var retVal = false;
			if (clause == Where.IfcAxis2Placement2D.RefDirIs2D) {
				try {
					retVal = (!(EXISTS(RefDirection))) || (RefDirection.Dim == 2);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcAxis2Placement2D");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAxis2Placement2D.RefDirIs2D' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAxis2Placement2D.LocationIs2D) {
				try {
					retVal = this/* as IfcPlacement*/.Location.Dim == 2;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcAxis2Placement2D");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAxis2Placement2D.LocationIs2D' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcAxis2Placement2D.RefDirIs2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement2D.RefDirIs2D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAxis2Placement2D.LocationIs2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement2D.LocationIs2D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcAxis2Placement2D
	{
		public static readonly IfcAxis2Placement2D RefDirIs2D = new IfcAxis2Placement2D();
		public static readonly IfcAxis2Placement2D LocationIs2D = new IfcAxis2Placement2D();
		protected IfcAxis2Placement2D() {}
	}
}
