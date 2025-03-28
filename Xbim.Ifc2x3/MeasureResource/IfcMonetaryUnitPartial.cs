using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.MeasureResource
{
    public partial class IfcMonetaryUnit
    {
        /// <summary>
        /// Gets the Symbol string for money unit
        /// </summary>
        /// <returns>String holding symbol</returns>
        public string Symbol
        {
            get
            {
                return this.Symbol();
            }
        }

        /// <summary>
        /// ets the name of the currency as its known internationally
        /// </summary>
        /// <returns>String as full name</returns>
        public string FullEnglishName
        {
            get
            {
                return this.FullEnglishName();
            }
        }

        /// <summary>
        /// Gets the name of the currency as its known natively
        /// </summary>
        /// <returns>String holding full name</returns>
        public string FullNativeName
        {
            get
            {
                return this.FullNativeName();
            }
        }

        public string FullName
        {
            get
            {
                return FullEnglishName;
            }
        }

    }
}
