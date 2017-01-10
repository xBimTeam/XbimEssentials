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
namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
	public partial struct IfcTextAlignment : IExpressValidatable
	{
		public enum IfcTextAlignmentClause
		{
			WR1,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcTextAlignmentClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTextAlignmentClause.WR1:
						retVal = NewArray("left", "right", "center", "justify").Contains(this);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationAppearanceResource.IfcTextAlignment");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTextAlignment.{0}'.", clause), ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcTextAlignmentClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTextAlignment.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
