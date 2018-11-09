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
namespace Xbim.Ifc2x3.Kernel
{
	public partial class IfcProject : IExpressValidatable
	{
		public enum IfcProjectClause
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
		public bool ValidateClause(IfcProjectClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcProjectClause.WR31:
						retVal = Functions.EXISTS(this/* as IfcRoot*/.Name);
						break;
					case IfcProjectClause.WR32:
						retVal = Functions.SIZEOF(RepresentationContexts.Where(Temp => Functions.TYPEOF(Temp).Contains("IFC2X3.IFCGEOMETRICREPRESENTATIONSUBCONTEXT"))) == 0;
						break;
					case IfcProjectClause.WR33:
						retVal = Functions.SIZEOF(this/* as IfcObjectDefinition*/.Decomposes) == 0;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.Kernel.IfcProject>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcProject.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcProjectClause.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProject.WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcProjectClause.WR32))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProject.WR32", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcProjectClause.WR33))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProject.WR33", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
