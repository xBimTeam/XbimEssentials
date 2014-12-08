#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPresentationLayerWithStyle.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationOrganizationResource
{
    ///<summary>
    ///  An IfcPresentationLayerAssignmentWithStyle extends the presentation layer assignment with capabilities to define visibility control, 
    ///  access control and common style information.
    ///  The visibility control allows to define a layer to be either 'on' or 'off', and/or 'frozen' or 'not frozen'. 
    ///  The access control allows to block graphical entities from manipulations by setting a layer to be either 'blocked' or 'not blocked'. 
    ///  Common style information can be given to the layer.
    ///
    ///  NOTE  Style information assigned to layers is often restricted to 'layer colour', 'curve font', and/or 'curve width'. 
    ///  These styles are assigned by using the IfcCurveStyle within the LayerStyles.
    ///
    ///  NOTE: If a styled item is assigned to a layer using the IfcPresentationLayerAssignmentWithStyle, it inherits the style information from the layer. 
    ///  In this case, it should omit its own style information. If the styled item has style information assigned 
    ///  (e.g. by IfcCurveStyle, IfcFillAreaStyle, IfcTextStyle, IfcSurfaceStyle, IfcSymbolStyle),  
    ///  then it overrides the style provided by the IfcPresentationLayerAssignmentWithStyle.
    ///</summary>
    [IfcPersistedEntityAttribute]
    public class IfcPresentationLayerWithStyle : IfcPresentationLayerAssignment
    {
        public IfcPresentationLayerWithStyle()
        {
            _layerStyles = new XbimSet<IfcPresentationStyleSelect>(this);
        }

        #region Fields

        private IfcLogical _layerOn;
        private IfcLogical _layerFrozen;
        private IfcLogical _layerBlocked;
        private XbimSet<IfcPresentationStyleSelect> _layerStyles;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   A logical setting, TRUE indicates that the layer is set to 'On', FALSE that the layer is set to 'Off', UNKNOWN that such information is not available.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcLogical LayerOn
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _layerOn;
            }
            set { this.SetModelValue(this, ref _layerOn, value, v => LayerOn = v, "LayerOn"); }
        }

        /// <summary>
        ///   A logical setting, TRUE indicates that the layer is set to 'Frozen', FALSE that the layer is set to 'Not frozen', UNKNOWN that such information is not available.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcLogical LayerFrozen
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _layerFrozen;
            }
            set { this.SetModelValue(this, ref _layerFrozen, value, v => LayerFrozen = v, "LayerFrozen"); }
        }

        /// <summary>
        ///   A logical setting, TRUE indicates that the layer is set to 'Blocked', FALSE that the layer is set to 'Not blocked', UNKNOWN that such information is not available.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcLogical LayerBlocked
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _layerBlocked;
            }
            set { this.SetModelValue(this, ref _layerBlocked, value, v => LayerBlocked = v, "LayerBlocked"); }
        }

        /// <summary>
        ///   A logical setting, TRUE indicates that the layer is set to 'Blocked', FALSE that the layer is set to 'Not blocked', UNKNOWN that such information is not available.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public XbimSet<IfcPresentationStyleSelect> LayerStyles
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _layerStyles;
            }
            set { this.SetModelValue(this, ref _layerStyles, value, v => LayerStyles = v, "LayerStyles"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _layerOn = value.BooleanVal;
                    break;
                case 5:
                    _layerFrozen = value.BooleanVal;
                    break;
                case 6:
                    _layerBlocked = value.BooleanVal;
                    break;
                case 7:
                    _layerStyles.Add((IfcPresentationStyleSelect) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}