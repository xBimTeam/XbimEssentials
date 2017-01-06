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
	public partial class IfcOffsetCurve3D : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcOffsetCurve3D");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcOffsetCurve3D clause) {
			var retVal = false;
			if (clause == Where.IfcOffsetCurve3D.DimIs2D) {
				try {
					retVal = BasisCurve.Dim == 3;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcOffsetCurve3D.DimIs2D' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcOffsetCurve3D.DimIs2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcOffsetCurve3D.DimIs2D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcOffsetCurve3D
	{
		public static readonly IfcOffsetCurve3D DimIs2D = new IfcOffsetCurve3D();
		protected IfcOffsetCurve3D() {}
	}
}
