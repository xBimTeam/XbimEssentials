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
namespace Xbim.Ifc4.Kernel
{
	public partial class IfcProject : IExpressValidatable
	{
		public enum IfcProjectClause
		{
			HasName,
			CorrectContext,
			NoDecomposition,
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
					case IfcProjectClause.HasName:
						retVal = Functions.EXISTS(this/* as IfcRoot*/.Name);
						break;
					case IfcProjectClause.CorrectContext:
						retVal = !(Functions.EXISTS(this/* as IfcContext*/.RepresentationContexts)) || (Functions.SIZEOF(this/* as IfcContext*/.RepresentationContexts.Where(Temp => Functions.TYPEOF(Temp).Contains("IFC4.IFCGEOMETRICREPRESENTATIONSUBCONTEXT"))) == 0);
						break;
					case IfcProjectClause.NoDecomposition:
						retVal = Functions.SIZEOF(this/* as IfcObjectDefinition*/.Decomposes) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.Kernel.IfcProject>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcProject.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcProjectClause.HasName))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProject.HasName", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcProjectClause.CorrectContext))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProject.CorrectContext", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcProjectClause.NoDecomposition))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProject.NoDecomposition", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
