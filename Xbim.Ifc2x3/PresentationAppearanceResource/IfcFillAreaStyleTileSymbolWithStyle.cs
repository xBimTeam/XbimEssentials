using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.PresentationDefinitionResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [IfcPersistedEntity]
    public class IfcFillAreaStyleTileSymbolWithStyle : IfcGeometricRepresentationItem, IfcFillAreaStyleTileShapeSelect
    {

        #region fields
        private IfcAnnotationSymbolOccurrence _symbol ;

        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcAnnotationSymbolOccurrence Symbol
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _symbol;
            }
            set { this.SetModelValue(this, ref _symbol, value, v => Symbol = v, "Symbol"); }
        }
        #endregion



        public override void IfcParse(int propIndex, XbimExtensions.Interfaces.IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _symbol = (IfcAnnotationSymbolOccurrence)value.EntityVal;
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
