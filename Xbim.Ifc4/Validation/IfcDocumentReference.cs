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
namespace Xbim.Ifc4.ExternalReferenceResource
{
	public partial class IfcDocumentReference : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcDocumentReference clause) {
			var retVal = false;
			if (clause == Where.IfcDocumentReference.WR1) {
				try {
					retVal = EXISTS(Name) ^ EXISTS(ReferencedDocument);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ExternalReferenceResource.IfcDocumentReference");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDocumentReference.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcExternalReference)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcDocumentReference.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDocumentReference.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcDocumentReference : IfcExternalReference
	{
		public new static readonly IfcDocumentReference WR1 = new IfcDocumentReference();
		protected IfcDocumentReference() {}
	}
}
