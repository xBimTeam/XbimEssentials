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
	public partial struct IfcFontStyle : IExpressValidatable
	{
		public enum IfcFontStyleClause
		{
			WR1,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcFontStyleClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcFontStyleClause.WR1:
						retVal = NewArray("normal", "italic", "oblique").Contains(this);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcFontStyle");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcFontStyle.{0}'.", clause), ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcFontStyleClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFontStyle.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
