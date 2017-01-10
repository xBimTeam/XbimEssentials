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
namespace Xbim.Ifc4.PresentationAppearanceResource
{
	public partial struct IfcTextTransformation : IExpressValidatable
	{
		public enum IfcTextTransformationClause
		{
			WR1,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcTextTransformationClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTextTransformationClause.WR1:
						retVal = NewArray("capitalize", "uppercase", "lowercase", "none").Contains(this);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcTextTransformation");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTextTransformation.{0}'.", clause), ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcTextTransformationClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTextTransformation.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
