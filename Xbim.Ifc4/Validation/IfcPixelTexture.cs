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
namespace Xbim.Ifc4.PresentationAppearanceResource
{
	public partial class IfcPixelTexture : IExpressValidatable
	{
		public enum IfcPixelTextureClause
		{
			MinPixelInS,
			MinPixelInT,
			NumberOfColours,
			SizeOfPixelList,
			PixelAsByteAndSameLength,
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
					case IfcPixelTextureClause.MinPixelInS:
						retVal = Width >= 1;
						break;
					case IfcPixelTextureClause.MinPixelInT:
						retVal = Height >= 1;
						break;
					case IfcPixelTextureClause.NumberOfColours:
						retVal = ((1 <= ColourComponents) && (ColourComponents <= 4) );
						break;
					case IfcPixelTextureClause.SizeOfPixelList:
						retVal = Functions.SIZEOF(Pixel) == (Width * Height);
						break;
					case IfcPixelTextureClause.PixelAsByteAndSameLength:
						retVal = Functions.SIZEOF(Pixel.Where(temp => (Functions.BLENGTH(temp) % 8 == 0) && (Functions.BLENGTH(temp) == Functions.BLENGTH(Pixel.ItemAt(0))))) == Functions.SIZEOF(Pixel);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.PresentationAppearanceResource.IfcPixelTexture>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcPixelTexture.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcPixelTextureClause.MinPixelInS))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.MinPixelInS", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPixelTextureClause.MinPixelInT))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.MinPixelInT", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPixelTextureClause.NumberOfColours))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.NumberOfColours", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPixelTextureClause.SizeOfPixelList))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.SizeOfPixelList", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPixelTextureClause.PixelAsByteAndSameLength))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.PixelAsByteAndSameLength", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
