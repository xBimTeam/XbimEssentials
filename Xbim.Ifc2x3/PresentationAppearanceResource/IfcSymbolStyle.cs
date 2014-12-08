using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    /// <summary>
    /// The symbol style is the presentation style that indicates the presentation of annotation symbols.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcSymbolStyle : IfcPresentationStyle, IfcPresentationStyleSelect
    {
        private IfcSymbolStyleSelect _StyleOfSymbol;

        /// <summary>
        /// The style applied to the symbol for its visual appearance. 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcSymbolStyleSelect StyleOfSymbol
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _StyleOfSymbol;
            }
            set { this.SetModelValue(this, ref _StyleOfSymbol, value, v => StyleOfSymbol = v, "StyleOfSymbol"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _StyleOfSymbol = (IfcSymbolStyleSelect)value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}
