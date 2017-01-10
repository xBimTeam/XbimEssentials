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
	public partial class IfcSweptSurface : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSweptSurface clause) {
			var retVal = false;
			if (clause == Where.IfcSweptSurface.SweptCurveType) {
				try {
					retVal = SweptCurve.ProfileType == IfcProfileTypeEnum.CURVE;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcSweptSurface");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSweptSurface.SweptCurveType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcSweptSurface.SweptCurveType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptSurface.SweptCurveType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcSweptSurface
	{
		public static readonly IfcSweptSurface SweptCurveType = new IfcSweptSurface();
		protected IfcSweptSurface() {}
	}
}
