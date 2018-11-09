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
namespace Xbim.Ifc2x3.FacilitiesMgmtDomain
{
	public partial class IfcMove : IExpressValidatable
	{
		public enum IfcMoveClause
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
		public bool ValidateClause(IfcMoveClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcMoveClause.WR1:
						retVal = Functions.SIZEOF(this/* as IfcProcess*/.OperatesOn) >= 1;
						break;
					case IfcMoveClause.WR2:
						retVal = Functions.SIZEOF(OperatesOn.Where(temp => Functions.SIZEOF(temp.RelatedObjects.Where(temp2 => (Functions.TYPEOF(temp2).Contains("IFC2X3.IFCACTOR")) || (Functions.TYPEOF(temp2).Contains("IFC2X3.IFCEQUIPMENTELEMENT")) || (Functions.TYPEOF(temp2).Contains("IFC2X3.IFCFURNISHINGELEMENT")))) >= 1)) >= 1;
						break;
					case IfcMoveClause.WR3:
						retVal = Functions.EXISTS(this/* as IfcRoot*/.Name);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.FacilitiesMgmtDomain.IfcMove>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcMove.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcMoveClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMove.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcMoveClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMove.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcMoveClause.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMove.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
