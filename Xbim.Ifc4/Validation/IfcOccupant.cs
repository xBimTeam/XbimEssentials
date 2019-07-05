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
namespace Xbim.Ifc4.SharedFacilitiesElements
{
	public partial class IfcOccupant : IExpressValidatable
	{
		public enum IfcOccupantClause
		{
			WR31,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcOccupantClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcOccupantClause.WR31:
						retVal = !(PredefinedType == IfcOccupantTypeEnum.USERDEFINED) || Functions.EXISTS(this/* as IfcObject*/.ObjectType);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.SharedFacilitiesElements.IfcOccupant>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcOccupant.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcOccupantClause.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcOccupant.WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
