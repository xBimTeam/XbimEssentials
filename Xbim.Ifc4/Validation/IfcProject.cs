using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
using static Xbim.Ifc4.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Kernel
{
	public partial class IfcProject : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcProject");

		/// <summary>
		/// Tests the express where clause HasName
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool HasName() {
			var retVal = false;
			try {
				retVal = EXISTS(this/* as IfcRoot*/.Name);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'HasName' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause CorrectContext
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrectContext() {
			var retVal = false;
			try {
				retVal = !(EXISTS(this/* as IfcContext*/.RepresentationContexts)) || (SIZEOF(this/* as IfcContext*/.RepresentationContexts.Where(Temp => TYPEOF(Temp).Contains("IFC4.IFCGEOMETRICREPRESENTATIONSUBCONTEXT"))) == 0);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrectContext' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause NoDecomposition
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NoDecomposition() {
			var retVal = false;
			try {
				retVal = SIZEOF(this/* as IfcObjectDefinition*/.Decomposes) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NoDecomposition' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!HasName())
				yield return new ValidationResult() { Item = this, IssueSource = "HasName", IssueType = ValidationFlags.EntityWhereClauses };
			if (!CorrectContext())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrectContext", IssueType = ValidationFlags.EntityWhereClauses };
			if (!NoDecomposition())
				yield return new ValidationResult() { Item = this, IssueSource = "NoDecomposition", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
