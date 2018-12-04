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
	public partial class IfcRectangleHollowProfileDef : IExpressValidatable
	{
		public enum IfcRectangleHollowProfileDefClause
		{
			WR31,
			WR32,
			WR33,
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
					case IfcRectangleHollowProfileDefClause.WR31:
						retVal = (WallThickness < (this/* as IfcRectangleProfileDef*/.XDim / 2)) && (WallThickness < (this/* as IfcRectangleProfileDef*/.YDim / 2));
						break;
					case IfcRectangleHollowProfileDefClause.WR32:
						retVal = !(Functions.EXISTS(OuterFilletRadius)) || ((OuterFilletRadius <= (this/* as IfcRectangleProfileDef*/.XDim / 2)) && (OuterFilletRadius <= (this/* as IfcRectangleProfileDef*/.YDim / 2)));
						break;
					case IfcRectangleHollowProfileDefClause.WR33:
						retVal = !(Functions.EXISTS(InnerFilletRadius)) || ((InnerFilletRadius <= (this/* as IfcRectangleProfileDef*/.XDim / 2 - WallThickness)) && (InnerFilletRadius <= (this/* as IfcRectangleProfileDef*/.YDim / 2 - WallThickness)));
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.ProfileResource.IfcRectangleHollowProfileDef>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRectangleHollowProfileDef.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcRectangleHollowProfileDefClause.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangleHollowProfileDef.WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRectangleHollowProfileDefClause.WR32))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangleHollowProfileDef.WR32", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRectangleHollowProfileDefClause.WR33))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangleHollowProfileDef.WR33", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
