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
	public partial class IfcRelDeclares : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcRelDeclares");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRelDeclares clause) {
			var retVal = false;
			if (clause == Where.IfcRelDeclares.NoSelfReference) {
				try {
					retVal = SIZEOF(RelatedDefinitions.Where(Temp => Object.ReferenceEquals(RelatingContext, Temp))) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRelDeclares.NoSelfReference' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcRelDeclares.NoSelfReference))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelDeclares.NoSelfReference", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcRelDeclares
	{
		public static readonly IfcRelDeclares NoSelfReference = new IfcRelDeclares();
		protected IfcRelDeclares() {}
	}
}
