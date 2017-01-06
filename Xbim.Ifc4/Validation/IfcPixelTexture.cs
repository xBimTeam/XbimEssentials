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
	public partial class IfcPixelTexture : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcPixelTexture");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPixelTexture clause) {
			var retVal = false;
			if (clause == Where.IfcPixelTexture.MinPixelInS) {
				try {
					retVal = Width >= 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPixelTexture.MinPixelInS' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPixelTexture.MinPixelInT) {
				try {
					retVal = Height >= 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPixelTexture.MinPixelInT' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPixelTexture.NumberOfColours) {
				try {
					retVal = ((1 <= ColourComponents) && (ColourComponents <= 4) );
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPixelTexture.NumberOfColours' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPixelTexture.SizeOfPixelList) {
				try {
					retVal = SIZEOF(Pixel) == (Width * Height);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPixelTexture.SizeOfPixelList' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPixelTexture.PixelAsByteAndSameLength) {
				try {
					retVal = SIZEOF(Pixel.Where(temp => (BLENGTH(temp) % 8 == 0) && (BLENGTH(temp) == BLENGTH(Pixel.ToArray()[0])))) == SIZEOF(Pixel);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPixelTexture.PixelAsByteAndSameLength' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPixelTexture.MinPixelInS))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.MinPixelInS", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPixelTexture.MinPixelInT))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.MinPixelInT", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPixelTexture.NumberOfColours))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.NumberOfColours", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPixelTexture.SizeOfPixelList))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.SizeOfPixelList", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPixelTexture.PixelAsByteAndSameLength))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPixelTexture.PixelAsByteAndSameLength", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPixelTexture
	{
		public static readonly IfcPixelTexture MinPixelInS = new IfcPixelTexture();
		public static readonly IfcPixelTexture MinPixelInT = new IfcPixelTexture();
		public static readonly IfcPixelTexture NumberOfColours = new IfcPixelTexture();
		public static readonly IfcPixelTexture SizeOfPixelList = new IfcPixelTexture();
		public static readonly IfcPixelTexture PixelAsByteAndSameLength = new IfcPixelTexture();
		protected IfcPixelTexture() {}
	}
}
