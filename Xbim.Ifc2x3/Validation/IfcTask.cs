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
namespace Xbim.Ifc2x3.ProcessExtension
{
	public partial class IfcTask : IExpressValidatable
	{
		public enum IfcTaskClause
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
		public bool ValidateClause(IfcTaskClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTaskClause.WR1:
						retVal = Functions.SIZEOF(this/* as IfcObjectDefinition*/.Decomposes.Where(temp => !(Functions.TYPEOF(temp).Contains("IFC2X3.IFCRELNESTS")))) == 0;
						break;
					case IfcTaskClause.WR2:
						retVal = Functions.SIZEOF(this/* as IfcObjectDefinition*/.IsDecomposedBy.Where(temp => !(Functions.TYPEOF(temp).Contains("IFC2X3.IFCRELNESTS")))) == 0;
						break;
					case IfcTaskClause.WR3:
						retVal = Functions.EXISTS(this/* as IfcRoot*/.Name);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.ProcessExtension.IfcTask>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcTask.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcTaskClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTask.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcTaskClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTask.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcTaskClause.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTask.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
