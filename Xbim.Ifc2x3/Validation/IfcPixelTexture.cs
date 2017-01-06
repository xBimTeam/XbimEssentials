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
	public partial class IfcPixelTexture : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationAppearanceResource.IfcPixelTexture");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPixelTexture clause) {
			var retVal = false;
			if (clause == Where.IfcPixelTexture.WR21) {
				try {
					retVal = Width >= 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPixelTexture.WR21' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPixelTexture.WR22) {
				try {
					retVal = Height >= 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPixelTexture.WR22' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPixelTexture.WR23) {
				try {
					retVal = ((1 <= ColourComponents) && (ColourComponents <= 4) );
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPixelTexture.WR23' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPixelTexture.WR24) {
				try {
					retVal = SIZEOF(Pixel) == (Width * Height);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPixelTexture.WR24' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPixelTexture.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPixelTexture.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.WR22", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPixelTexture.WR23))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.WR23", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPixelTexture.WR24))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.WR24", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcPixelTexture
	{
		public static readonly IfcPixelTexture WR21 = new IfcPixelTexture();
		public static readonly IfcPixelTexture WR22 = new IfcPixelTexture();
		public static readonly IfcPixelTexture WR23 = new IfcPixelTexture();
		public static readonly IfcPixelTexture WR24 = new IfcPixelTexture();
		protected IfcPixelTexture() {}
	}
}
