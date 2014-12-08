using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.ProfilePropertyResource
{
    /// <summary>
    /// Instances of the entity IfcRibPlateProfileProperties shall be used for a parameterized definition of rib plates.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcRibPlateProfileProperties : IfcProfileProperties
    {
        private IfcPositiveLengthMeasure? _Thickness;

        /// <summary>
        /// Defines the thickness of the structural face member. 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? Thickness
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Thickness;
            }
            set { this.SetModelValue(this, ref _Thickness, value, v => Thickness = v, "Thickness"); }
        }

        private IfcPositiveLengthMeasure? _RibHeight;

        /// <summary>
        /// Height of the ribs. 
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? RibHeight
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _RibHeight;
            }
            set { this.SetModelValue(this, ref _RibHeight, value, v => RibHeight = v, "RibHeight"); }
        }

        private IfcPositiveLengthMeasure? _RibWidth;

        /// <summary>
        /// Width of the ribs. 
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? RibWidth
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _RibWidth;
            }
            set { this.SetModelValue(this, ref _RibWidth, value, v => RibWidth = v, "RibWidth"); }
        }

        private IfcPositiveLengthMeasure? _RibSpacing;

        /// <summary>
        ///  Spacing between the axes of the ribs. 
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? RibSpacing
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _RibSpacing;
            }
            set { this.SetModelValue(this, ref _RibSpacing, value, v => RibSpacing = v, "RibSpacing"); }
        }

        private IfcRibPlateDirectionEnum _Direction;

        /// <summary>
        ///  Defines the direction of profile definition as described on figure above. 
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcRibPlateDirectionEnum Direction
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Direction;
            }
            set { this.SetModelValue(this, ref _Direction, value, v => Direction = v, "Direction"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                    base.IfcParse(propIndex, value);
                    break;
                case 2:
                    _Thickness = value.RealVal;
                    break;
                case 3:
                    _RibHeight = value.RealVal;
                    break;
                case 4:
                    _RibWidth = value.RealVal;
                    break;
                case 5:
                    _RibSpacing = value.RealVal;
                    break;
                case 6:
                    _Direction = (IfcRibPlateDirectionEnum)Enum.Parse(typeof(IfcRibPlateDirectionEnum), value.EnumVal);
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
