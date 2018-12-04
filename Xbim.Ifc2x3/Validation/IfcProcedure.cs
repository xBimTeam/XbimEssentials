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
	public partial class IfcProcedure : IExpressValidatable
	{
		public enum IfcProcedureClause
		{
			WR1,
			WR2,
			WR3,
			WR4,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcProcedureClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcProcedureClause.WR1:
						retVal = Functions.SIZEOF(this/* as IfcObjectDefinition*/.Decomposes.Where(temp => !(Functions.TYPEOF(temp).Contains("IFC2X3.IFCRELNESTS")))) == 0;
						break;
					case IfcProcedureClause.WR2:
						retVal = Functions.SIZEOF(this/* as IfcObjectDefinition*/.IsDecomposedBy.Where(temp => !(Functions.TYPEOF(temp).Contains("IFC2X3.IFCRELNESTS")))) == 0;
						break;
					case IfcProcedureClause.WR3:
						retVal = Functions.EXISTS(this/* as IfcRoot*/.Name);
						break;
					case IfcProcedureClause.WR4:
						retVal = (ProcedureType != IfcProcedureTypeEnum.USERDEFINED) || ((ProcedureType == IfcProcedureTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcProcedure*/.UserDefinedProcedureType));
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.ProcessExtension.IfcProcedure>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcProcedure.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcProcedureClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProcedure.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcProcedureClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProcedure.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcProcedureClause.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProcedure.WR3", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcProcedureClause.WR4))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProcedure.WR4", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
