using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.GeometryResource
{
	public partial class IfcAxis2Placement2D : IExpressValidatable
	{
		public enum IfcAxis2Placement2DClause
		{
			RefDirIs2D,
			LocationIs2D,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcAxis2Placement2DClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcAxis2Placement2DClause.RefDirIs2D:
						retVal = (!(Functions.EXISTS(RefDirection))) || (RefDirection.Dim == 2);
						break;
					case IfcAxis2Placement2DClause.LocationIs2D:
						retVal = this/* as IfcPlacement*/.Location.Dim == 2;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcAxis2Placement2D");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAxis2Placement2D.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcAxis2Placement2DClause.RefDirIs2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement2D.RefDirIs2D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAxis2Placement2DClause.LocationIs2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement2D.LocationIs2D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
