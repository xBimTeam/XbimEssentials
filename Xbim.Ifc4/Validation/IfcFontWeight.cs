using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.PresentationAppearanceResource
{
	public partial struct IfcFontWeight : IExpressValidatable
	{
		public enum IfcFontWeightClause
		{
			WR1,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcFontWeightClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcFontWeightClause.WR1:
						retVal = Functions.NewArray("normal", "small-caps", "100", "200", "300", "400", "500", "600", "700", "800", "900").Contains(this);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcFontWeight");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcFontWeight.{0}'.", clause), ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcFontWeightClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFontWeight.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
