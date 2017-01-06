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
	public partial class IfcPolyline : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcPolyline");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPolyline clause) {
			var retVal = false;
			if (clause == Where.IfcPolyline.SameDim) {
				try {
					retVal = SIZEOF(Points.Where(Temp => Temp.Dim != Points.ToArray()[0].Dim)) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPolyline.SameDim' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPolyline.SameDim))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPolyline.SameDim", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPolyline
	{
		public static readonly IfcPolyline SameDim = new IfcPolyline();
		protected IfcPolyline() {}
	}
}
