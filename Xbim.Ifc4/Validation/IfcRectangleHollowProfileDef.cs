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
namespace Xbim.Ifc4.ProfileResource
{
	public partial class IfcRectangleHollowProfileDef : IExpressValidatable
	{
		public enum IfcRectangleHollowProfileDefClause
		{
			ValidWallThickness,
			ValidInnerRadius,
			ValidOuterRadius,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRectangleHollowProfileDefClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRectangleHollowProfileDefClause.ValidWallThickness:
						retVal = (WallThickness < (this/* as IfcRectangleProfileDef*/.XDim / 2)) && (WallThickness < (this/* as IfcRectangleProfileDef*/.YDim / 2));
						break;
					case IfcRectangleHollowProfileDefClause.ValidInnerRadius:
						retVal = !(Functions.EXISTS(InnerFilletRadius)) || ((InnerFilletRadius <= (this/* as IfcRectangleProfileDef*/.XDim / 2 - WallThickness)) && (InnerFilletRadius <= (this/* as IfcRectangleProfileDef*/.YDim / 2 - WallThickness)));
						break;
					case IfcRectangleHollowProfileDefClause.ValidOuterRadius:
						retVal = !(Functions.EXISTS(OuterFilletRadius)) || ((OuterFilletRadius <= (this/* as IfcRectangleProfileDef*/.XDim / 2)) && (OuterFilletRadius <= (this/* as IfcRectangleProfileDef*/.YDim / 2)));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProfileResource.IfcRectangleHollowProfileDef>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRectangleHollowProfileDef.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcRectangleHollowProfileDefClause.ValidWallThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangleHollowProfileDef.ValidWallThickness", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRectangleHollowProfileDefClause.ValidInnerRadius))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangleHollowProfileDef.ValidInnerRadius", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRectangleHollowProfileDefClause.ValidOuterRadius))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangleHollowProfileDef.ValidOuterRadius", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
