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
	public partial class IfcExtrudedAreaSolid : IExpressValidatable
	{
		public enum IfcExtrudedAreaSolidClause
		{
			ValidExtrusionDirection,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcExtrudedAreaSolidClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcExtrudedAreaSolidClause.ValidExtrusionDirection:
						retVal = IfcDotProduct(IfcDirection(0, 0, 1), this.ExtrudedDirection) != 0;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcExtrudedAreaSolid");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcExtrudedAreaSolid.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcExtrudedAreaSolidClause.ValidExtrusionDirection))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcExtrudedAreaSolid.ValidExtrusionDirection", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
