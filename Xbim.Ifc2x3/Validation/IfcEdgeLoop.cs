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
namespace Xbim.Ifc2x3.TopologyResource
{
	public partial class IfcEdgeLoop : IExpressValidatable
	{
		public enum IfcEdgeLoopClause
		{
			WR1,
			WR2,
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
					case IfcEdgeLoopClause.WR1:
						retVal = Object.ReferenceEquals((EdgeList.ItemAt(0).EdgeStart), (EdgeList.ItemAt(Ne-1).EdgeEnd));
						break;
					case IfcEdgeLoopClause.WR2:
						retVal = Functions.IfcLoopHeadToTail(this);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.TopologyResource.IfcEdgeLoop>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcEdgeLoop.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcEdgeLoopClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEdgeLoop.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcEdgeLoopClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEdgeLoop.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
