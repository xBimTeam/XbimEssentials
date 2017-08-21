using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.ProfileResource
{
	public partial class IfcCShapeProfileDef : IExpressValidatable
	{
		public enum IfcCShapeProfileDefClause
		{
			WR1,
			WR2,
			WR3,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCShapeProfileDefClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCShapeProfileDefClause.WR1:
						retVal = Girth < (Depth / 2);
						break;
					case IfcCShapeProfileDefClause.WR2:
						retVal = !(Functions.EXISTS(InternalFilletRadius)) || ((InternalFilletRadius <= Width / 2) && (InternalFilletRadius <= Depth / 2));
						break;
					case IfcCShapeProfileDefClause.WR3:
						retVal = (WallThickness < Width / 2) && (WallThickness < Depth / 2);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.ProfileResource.IfcCShapeProfileDef>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcCShapeProfileDef.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcCShapeProfileDefClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCShapeProfileDef.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcCShapeProfileDefClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCShapeProfileDef.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcCShapeProfileDefClause.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCShapeProfileDef.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
