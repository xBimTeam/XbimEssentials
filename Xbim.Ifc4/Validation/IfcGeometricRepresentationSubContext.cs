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
namespace Xbim.Ifc4.RepresentationResource
{
	public partial class IfcGeometricRepresentationSubContext : IExpressValidatable
	{
		public enum IfcGeometricRepresentationSubContextClause
		{
			ParentNoSub,
			UserTargetProvided,
			NoCoordOperation,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcGeometricRepresentationSubContextClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcGeometricRepresentationSubContextClause.ParentNoSub:
						retVal = !(Functions.TYPEOF(ParentContext).Contains("IFC4.IFCGEOMETRICREPRESENTATIONSUBCONTEXT"));
						break;
					case IfcGeometricRepresentationSubContextClause.UserTargetProvided:
						retVal = (TargetView != IfcGeometricProjectionEnum.USERDEFINED) || ((TargetView == IfcGeometricProjectionEnum.USERDEFINED) && Functions.EXISTS(UserDefinedTargetView));
						break;
					case IfcGeometricRepresentationSubContextClause.NoCoordOperation:
						retVal = Functions.SIZEOF(this/* as IfcGeometricRepresentationContext*/.HasCoordinateOperation) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.RepresentationResource.IfcGeometricRepresentationSubContext>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcGeometricRepresentationSubContext.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcGeometricRepresentationSubContextClause.ParentNoSub))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGeometricRepresentationSubContext.ParentNoSub", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcGeometricRepresentationSubContextClause.UserTargetProvided))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGeometricRepresentationSubContext.UserTargetProvided", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcGeometricRepresentationSubContextClause.NoCoordOperation))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGeometricRepresentationSubContext.NoCoordOperation", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
