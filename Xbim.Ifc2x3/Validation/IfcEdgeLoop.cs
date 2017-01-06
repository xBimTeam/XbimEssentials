using System;
using log4net;
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
using static Xbim.Ifc2x3.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.TopologyResource
{
	public partial class IfcEdgeLoop : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.TopologyResource.IfcEdgeLoop");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcEdgeLoop clause) {
			var retVal = false;
			if (clause == Where.IfcEdgeLoop.WR1) {
				try {
					retVal = Object.ReferenceEquals((EdgeList.ToArray()[0].EdgeStart), (EdgeList.ToArray()[Ne-1].EdgeEnd));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcEdgeLoop.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcEdgeLoop.WR2) {
				try {
					retVal = IfcLoopHeadToTail(this);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcEdgeLoop.WR2' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcEdgeLoop.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEdgeLoop.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcEdgeLoop.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcEdgeLoop.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcEdgeLoop
	{
		public static readonly IfcEdgeLoop WR1 = new IfcEdgeLoop();
		public static readonly IfcEdgeLoop WR2 = new IfcEdgeLoop();
		protected IfcEdgeLoop() {}
	}
}
