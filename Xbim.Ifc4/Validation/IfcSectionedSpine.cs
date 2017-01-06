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
namespace Xbim.Ifc4.GeometricModelResource
{
	public partial class IfcSectionedSpine : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcSectionedSpine");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSectionedSpine clause) {
			var retVal = false;
			if (clause == Where.IfcSectionedSpine.CorrespondingSectionPositions) {
				try {
					retVal = SIZEOF(CrossSections) == SIZEOF(CrossSectionPositions);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSectionedSpine.CorrespondingSectionPositions' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSectionedSpine.ConsistentProfileTypes) {
				try {
					retVal = SIZEOF(CrossSections.Where(temp => CrossSections.ToArray()[0].ProfileType != temp.ProfileType)) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSectionedSpine.ConsistentProfileTypes' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSectionedSpine.SpineCurveDim) {
				try {
					retVal = SpineCurve.Dim == 3;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSectionedSpine.SpineCurveDim' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcSectionedSpine.CorrespondingSectionPositions))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSectionedSpine.CorrespondingSectionPositions", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSectionedSpine.ConsistentProfileTypes))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSectionedSpine.ConsistentProfileTypes", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSectionedSpine.SpineCurveDim))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSectionedSpine.SpineCurveDim", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcSectionedSpine
	{
		public static readonly IfcSectionedSpine CorrespondingSectionPositions = new IfcSectionedSpine();
		public static readonly IfcSectionedSpine ConsistentProfileTypes = new IfcSectionedSpine();
		public static readonly IfcSectionedSpine SpineCurveDim = new IfcSectionedSpine();
		protected IfcSectionedSpine() {}
	}
}
