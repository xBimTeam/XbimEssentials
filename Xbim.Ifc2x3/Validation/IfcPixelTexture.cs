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
namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
	public partial class IfcPixelTexture : IExpressValidatable
	{
		public enum IfcPixelTextureClause
		{
			WR21,
			WR22,
			WR23,
			WR24,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcPixelTextureClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcPixelTextureClause.WR21:
						retVal = Width >= 1;
						break;
					case IfcPixelTextureClause.WR22:
						retVal = Height >= 1;
						break;
					case IfcPixelTextureClause.WR23:
						retVal = ((1 <= ColourComponents) && (ColourComponents <= 4) );
						break;
					case IfcPixelTextureClause.WR24:
						retVal = Functions.SIZEOF(Pixel) == (Width * Height);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.PresentationAppearanceResource.IfcPixelTexture>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcPixelTexture.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcPixelTextureClause.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPixelTextureClause.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.WR22", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPixelTextureClause.WR23))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.WR23", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPixelTextureClause.WR24))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.WR24", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
