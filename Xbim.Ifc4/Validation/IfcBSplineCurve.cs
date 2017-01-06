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
	public partial class IfcBSplineCurve : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcBSplineCurve");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcBSplineCurve clause) {
			var retVal = false;
			if (clause == Where.IfcBSplineCurve.SameDim) {
				try {
					retVal = SIZEOF(ControlPointsList.Where(Temp => Temp.Dim != ControlPointsList.ToArray()[0].Dim)) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcBSplineCurve.SameDim' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcBSplineCurve.SameDim))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBSplineCurve.SameDim", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcBSplineCurve
	{
		public static readonly IfcBSplineCurve SameDim = new IfcBSplineCurve();
		protected IfcBSplineCurve() {}
	}
}
