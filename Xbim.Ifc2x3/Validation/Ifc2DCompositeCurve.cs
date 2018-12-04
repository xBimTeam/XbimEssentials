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
namespace Xbim.Ifc2x3.GeometryResource
{
	public partial class Ifc2DCompositeCurve : IExpressValidatable
	{
		public enum Ifc2DCompositeCurveClause
		{
			WR1,
			WR2,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Ifc2DCompositeCurveClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case Ifc2DCompositeCurveClause.WR1:
						retVal = this/* as IfcCompositeCurve*/.ClosedCurve.Value;
						break;
					case Ifc2DCompositeCurveClause.WR2:
						retVal = this/* as IfcCurve*/.Dim == 2;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.GeometryResource.Ifc2DCompositeCurve>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'Ifc2DCompositeCurve.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Ifc2DCompositeCurveClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "Ifc2DCompositeCurve.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Ifc2DCompositeCurveClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "Ifc2DCompositeCurve.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
