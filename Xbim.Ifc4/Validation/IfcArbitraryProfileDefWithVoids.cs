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
	public partial class IfcArbitraryProfileDefWithVoids : IExpressValidatable
	{
		public enum IfcArbitraryProfileDefWithVoidsClause
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
		public bool ValidateClause(IfcArbitraryProfileDefWithVoidsClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcArbitraryProfileDefWithVoidsClause.WR1:
						retVal = this/* as IfcProfileDef*/.ProfileType == IfcProfileTypeEnum.AREA;
						break;
					case IfcArbitraryProfileDefWithVoidsClause.WR2:
						retVal = Functions.SIZEOF(InnerCurves.Where(temp => temp.Dim != 2)) == 0;
						break;
					case IfcArbitraryProfileDefWithVoidsClause.WR3:
						retVal = Functions.SIZEOF(InnerCurves.Where(temp => Functions.TYPEOF(temp).Contains("IFC4.IFCLINE"))) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProfileResource.IfcArbitraryProfileDefWithVoids>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcArbitraryProfileDefWithVoids.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcArbitraryProfileDefWithVoidsClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcArbitraryProfileDefWithVoids.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcArbitraryProfileDefWithVoidsClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcArbitraryProfileDefWithVoids.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcArbitraryProfileDefWithVoidsClause.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcArbitraryProfileDefWithVoids.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
