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
	public partial class IfcRelAssignsToResource : IExpressValidatable
	{
		public enum IfcRelAssignsToResourceClause
		{
			NoSelfReference,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRelAssignsToResourceClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRelAssignsToResourceClause.NoSelfReference:
						retVal = SIZEOF(this/* as IfcRelAssigns*/.RelatedObjects.Where(Temp => Object.ReferenceEquals(RelatingResource, Temp))) == 0;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcRelAssignsToResource");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcRelAssignsToResource.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcRelAssignsToResourceClause.NoSelfReference))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelAssignsToResource.NoSelfReference", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
