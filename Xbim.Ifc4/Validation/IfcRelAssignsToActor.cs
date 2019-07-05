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
	public partial class IfcRelAssignsToActor : IExpressValidatable
	{
		public enum IfcRelAssignsToActorClause
		{
			NoSelfReference,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRelAssignsToActorClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRelAssignsToActorClause.NoSelfReference:
						retVal = Functions.SIZEOF(this/* as IfcRelAssigns*/.RelatedObjects.Where(Temp => Object.ReferenceEquals(RelatingActor, Temp))) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.Kernel.IfcRelAssignsToActor>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRelAssignsToActor.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcRelAssignsToActorClause.NoSelfReference))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelAssignsToActor.NoSelfReference", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
