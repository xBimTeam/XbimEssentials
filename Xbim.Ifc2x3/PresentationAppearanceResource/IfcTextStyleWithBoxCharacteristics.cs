using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    /// <summary>
    /// The text style with box characteristics allows the presentation of annotated text by specifying the characteristics of the character boxes of the text and the spacing between the character boxes.
    /// 
    ///     NOTE  The IfcTextStyleWithBoxCharacteristics is an entity that had been adopted from ISO 10303, Industrial automation systems and integration—Product data representation and exchange, Part 46: Integrated generic resources: Visual presentation. 
    /// 
    /// The IfcTextStyleWithBoxCharacteristics is mainly used to provide some compatibility with ISO10303. Its usage is restricted to monospace text fonts (having uniform character boxes) and simple vector based text fonts. For true text fonts however the use of IfcTextStyleTextModel is required.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcTextStyleWithBoxCharacteristics : IfcTextStyleSelect
    {
        private IfcPositiveLengthMeasure? _BoxHeight;

        /// <summary>
        /// It is the height scaling factor in the definition of a character glyph. 
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? BoxHeight
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _BoxHeight;
            }
            set { this.SetModelValue(this, ref _BoxHeight, value, v => BoxHeight = v, "BoxHeight"); }
        }

        private IfcPositiveLengthMeasure? _BoxWidth;

        /// <summary>
        /// It is the width scaling factor in the definition of a character glyph. 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? BoxWidth
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _BoxWidth;
            }
            set { this.SetModelValue(this, ref _BoxWidth, value, v => BoxWidth = v, "BoxWidth"); }
        }

        private IfcPlaneAngleMeasure? _BoxSlantAngle;

        /// <summary>
        /// It indicated that the box of a character glyph shall be represented as a parallelogram, with the angle being between the character up line and an axis perpendicular to the character base line. 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcPlaneAngleMeasure? BoxSlantAngle
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _BoxSlantAngle;
            }
            set { this.SetModelValue(this, ref _BoxSlantAngle, value, v => BoxSlantAngle = v, "BoxSlantAngle"); }
        }

        private IfcPlaneAngleMeasure? _BoxRotateAngle;

        /// <summary>
        /// It indicated that the box of a character glyph shall be presented at an angle to the base line of a text string within which the glyph occurs, the angle being that between the base line of the glyph and an axis perpendicular to the baseline of the text string. 
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcPlaneAngleMeasure? BoxRotateAngle
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _BoxRotateAngle;
            }
            set { this.SetModelValue(this, ref _BoxRotateAngle, value, v => BoxRotateAngle = v, "BoxRotateAngle"); }
        }

        private IfcSizeSelect _CharacterSpacing;

        /// <summary>
        ///  The distance between the character boxes of adjacent characters. 
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcSizeSelect CharacterSpacing
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _CharacterSpacing;
            }
            set { this.SetModelValue(this, ref _CharacterSpacing, value, v => CharacterSpacing = v, "CharacterSpacing"); }
        }

        void XbimExtensions.Interfaces.IPersistIfc.IfcParse(int propIndex, XbimExtensions.Interfaces.IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _BoxHeight = value.RealVal;
                    break;
                case 1:
                    _BoxWidth = value.RealVal;
                    break;
                case 2:
                    _BoxSlantAngle = value.RealVal;
                    break;
                case 3:
                    _BoxRotateAngle = value.RealVal;
                    break;
                case 4:
                    _CharacterSpacing = (IfcSizeSelect)value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        string XbimExtensions.Interfaces.IPersistIfc.WhereRule()
        {
            return "";
        }

        private IModel _model;
        private int _entityLabel;
        private bool _activated;

        bool IPersistIfcEntity.Activated
        {
            get { return _activated; }
        }

        void IPersistIfcEntity.Activate(bool write)
        {
            lock (this) { if (_model != null && !_activated) _activated = _model.Activate(this, false) > 0; }
            if (write) _model.Activate(this, write);
        }

        void IPersistIfcEntity.Bind(IModel model, int entityLabel, bool activated)
        {
            _model = model;
            _entityLabel = entityLabel;
            _activated = activated;
        }

        IModel IPersistIfcEntity.ModelOf
        {
            get { return _model; }
        }

        int IPersistIfcEntity.EntityLabel
        {
            get { return _entityLabel; }
        }

        [field: NonSerialized] //don't serialize events
        private event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized] //don't serialize event
        private event PropertyChangingEventHandler PropertyChanging;

        void ISupportChangeNotification.NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        void ISupportChangeNotification.NotifyPropertyChanging(string propertyName)
        {
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        event System.ComponentModel.PropertyChangedEventHandler System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        event System.ComponentModel.PropertyChangingEventHandler System.ComponentModel.INotifyPropertyChanging.PropertyChanging
        {
            add { PropertyChanging += value; }
            remove { PropertyChanging -= value; }
        }
    }
}
