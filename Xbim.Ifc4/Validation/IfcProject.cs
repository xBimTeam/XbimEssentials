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
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcProject clause) {
			var retVal = false;
			if (clause == Where.IfcProject.HasName) {
				try {
					retVal = EXISTS(this/* as IfcRoot*/.Name);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcProject.HasName' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcProject.CorrectContext) {
				try {
					retVal = !(EXISTS(this/* as IfcContext*/.RepresentationContexts)) || (SIZEOF(this/* as IfcContext*/.RepresentationContexts.Where(Temp => TYPEOF(Temp).Contains("IFC4.IFCGEOMETRICREPRESENTATIONSUBCONTEXT"))) == 0);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcProject.CorrectContext' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcProject.NoDecomposition) {
				try {
					retVal = SIZEOF(this/* as IfcObjectDefinition*/.Decomposes) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcProject.NoDecomposition' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcProject.HasName))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProject.HasName", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcProject.CorrectContext))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProject.CorrectContext", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcProject.NoDecomposition))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProject.NoDecomposition", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcProject
	{
		public static readonly IfcProject HasName = new IfcProject();
		public static readonly IfcProject CorrectContext = new IfcProject();
		public static readonly IfcProject NoDecomposition = new IfcProject();
		protected IfcProject() {}
	}
}
