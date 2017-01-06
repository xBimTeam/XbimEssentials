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
	public partial class IfcCartesianPoint : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcCartesianPoint");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCartesianPoint clause) {
			var retVal = false;
			if (clause == Where.IfcCartesianPoint.CP2Dor3D) {
				try {
					retVal = HIINDEX(Coordinates) >= 2;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCartesianPoint.CP2Dor3D' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcCartesianPoint.CP2Dor3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianPoint.CP2Dor3D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcCartesianPoint
	{
		public static readonly IfcCartesianPoint CP2Dor3D = new IfcCartesianPoint();
		protected IfcCartesianPoint() {}
	}
}
