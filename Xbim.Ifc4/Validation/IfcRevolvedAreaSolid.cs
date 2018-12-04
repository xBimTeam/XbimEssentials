using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.GeometricModelResource
{
	public partial class IfcRevolvedAreaSolid : IExpressValidatable
	{
		public enum IfcRevolvedAreaSolidClause
		{
			AxisStartInXY,
			AxisDirectionInXY,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRevolvedAreaSolidClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRevolvedAreaSolidClause.AxisStartInXY:
						retVal = Axis.Location.Coordinates.ItemAt(2) == 0;
						break;
					case IfcRevolvedAreaSolidClause.AxisDirectionInXY:
						retVal = Axis.Z.DirectionRatios().ItemAt(2) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometricModelResource.IfcRevolvedAreaSolid>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRevolvedAreaSolid.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcRevolvedAreaSolidClause.AxisStartInXY))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRevolvedAreaSolid.AxisStartInXY", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRevolvedAreaSolidClause.AxisDirectionInXY))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRevolvedAreaSolid.AxisDirectionInXY", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
