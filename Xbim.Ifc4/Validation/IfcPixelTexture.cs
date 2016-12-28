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
		/// Tests the express where clause MinPixelInS
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MinPixelInS() {
			var retVal = false;
			try {
				retVal = Width >= 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MinPixelInS' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause MinPixelInT
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MinPixelInT() {
			var retVal = false;
			try {
				retVal = Height >= 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MinPixelInT' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause NumberOfColours
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NumberOfColours() {
			var retVal = false;
			try {
				retVal = ((1 <= ColourComponents) && (ColourComponents <= 4) );
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NumberOfColours' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause SizeOfPixelList
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool SizeOfPixelList() {
			var retVal = false;
			try {
				retVal = SIZEOF(Pixel) == (Width * Height);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'SizeOfPixelList' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause PixelAsByteAndSameLength
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool PixelAsByteAndSameLength() {
			var retVal = false;
			try {
				retVal = SIZEOF(Pixel.Where(temp => (BLENGTH(temp) % 8 == 0) && (BLENGTH(temp) == BLENGTH(Pixel.ToArray()[0])))) == SIZEOF(Pixel);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'PixelAsByteAndSameLength' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!MinPixelInS())
				yield return new ValidationResult() { Item = this, IssueSource = "MinPixelInS", IssueType = ValidationFlags.EntityWhereClauses };
			if (!MinPixelInT())
				yield return new ValidationResult() { Item = this, IssueSource = "MinPixelInT", IssueType = ValidationFlags.EntityWhereClauses };
			if (!NumberOfColours())
				yield return new ValidationResult() { Item = this, IssueSource = "NumberOfColours", IssueType = ValidationFlags.EntityWhereClauses };
			if (!SizeOfPixelList())
				yield return new ValidationResult() { Item = this, IssueSource = "SizeOfPixelList", IssueType = ValidationFlags.EntityWhereClauses };
			if (!PixelAsByteAndSameLength())
				yield return new ValidationResult() { Item = this, IssueSource = "PixelAsByteAndSameLength", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
