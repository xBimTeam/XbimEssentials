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
	public partial class IfcRelAssignsToControl : IExpressValidatable
	{
		public enum IfcRelAssignsToControlClause
		{
			NoSelfReference,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRelAssignsToControlClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRelAssignsToControlClause.NoSelfReference:
						retVal = SIZEOF(this/* as IfcRelAssigns*/.RelatedObjects.Where(Temp => Object.ReferenceEquals(RelatingControl, Temp))) == 0;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcRelAssignsToControl");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcRelAssignsToControl.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcRelAssignsToControlClause.NoSelfReference))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelAssignsToControl.NoSelfReference", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
