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
	public partial class IfcOffsetCurve2D : IExpressValidatable
	{
		public enum IfcOffsetCurve2DClause
		{
			DimIs2D,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcOffsetCurve2DClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcOffsetCurve2DClause.DimIs2D:
						retVal = BasisCurve.Dim == 2;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcOffsetCurve2D");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcOffsetCurve2D.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcOffsetCurve2DClause.DimIs2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcOffsetCurve2D.DimIs2D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
