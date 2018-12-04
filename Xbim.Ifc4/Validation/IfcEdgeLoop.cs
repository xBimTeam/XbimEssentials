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
namespace Xbim.Ifc4.TopologyResource
{
	public partial class IfcEdgeLoop : IExpressValidatable
	{
		public enum IfcEdgeLoopClause
		{
			IsClosed,
			IsContinuous,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcEdgeLoopClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcEdgeLoopClause.IsClosed:
						retVal = Object.ReferenceEquals((EdgeList.ItemAt(0).EdgeStart), (EdgeList.ItemAt(Ne-1).EdgeEnd));
						break;
					case IfcEdgeLoopClause.IsContinuous:
						retVal = Functions.IfcLoopHeadToTail(this);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.TopologyResource.IfcEdgeLoop>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcEdgeLoop.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcEdgeLoopClause.IsClosed))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEdgeLoop.IsClosed", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcEdgeLoopClause.IsContinuous))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEdgeLoop.IsContinuous", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
