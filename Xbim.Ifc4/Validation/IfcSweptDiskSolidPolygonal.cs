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
namespace Xbim.Ifc4.GeometricModelResource
{
	public partial class IfcSweptDiskSolidPolygonal : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcSweptDiskSolidPolygonal");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSweptDiskSolidPolygonal clause) {
			var retVal = false;
			if (clause == Where.IfcSweptDiskSolidPolygonal.CorrectRadii) {
				try {
					retVal = !(EXISTS(FilletRadius)) || (FilletRadius >= this/* as IfcSweptDiskSolid*/.Radius);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSweptDiskSolidPolygonal.CorrectRadii' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSweptDiskSolidPolygonal.DirectrixIsPolyline) {
				try {
					retVal = TYPEOF(this/* as IfcSweptDiskSolid*/.Directrix).Contains("IFC4.IFCPOLYLINE");
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSweptDiskSolidPolygonal.DirectrixIsPolyline' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcSweptDiskSolid)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcSweptDiskSolidPolygonal.CorrectRadii))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptDiskSolidPolygonal.CorrectRadii", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSweptDiskSolidPolygonal.DirectrixIsPolyline))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptDiskSolidPolygonal.DirectrixIsPolyline", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcSweptDiskSolidPolygonal : IfcSweptDiskSolid
	{
		public static readonly IfcSweptDiskSolidPolygonal CorrectRadii = new IfcSweptDiskSolidPolygonal();
		public static readonly IfcSweptDiskSolidPolygonal DirectrixIsPolyline = new IfcSweptDiskSolidPolygonal();
		protected IfcSweptDiskSolidPolygonal() {}
	}
}
