#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcExtendedMaterialProperties.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.PropertyResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.MaterialPropertyResource
{
    public class IfcExtendedMaterialProperties : IfcMaterialProperties
    {
        #region Fields 

        private SetOfProperty _extendedProperties;
        private IfcText? _description;
        private IfcLabel _name;

        #endregion

        public IfcExtendedMaterialProperties() //???
        {
            //???
            _extendedProperties = new SetOfProperty(this); //???
        }

        /// <summary>
        ///   The set of material properties defined by the user for this material
        /// </summary>
        [Ifc(2, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        public SetOfProperty ExtendedProperties
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _extendedProperties;
            }
            set
            {
                this.SetModelValue(this, ref _extendedProperties, value, v => ExtendedProperties = v,
                                           "ExtendedProperties");
            }
        }

        /// <summary>
        ///   Optional Description for the set of extended properties.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcText? Description
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _description;
            }
            set { this.SetModelValue(this, ref _description, value, v => Description = v, "Description"); }
        }

        /// <summary>
        ///   The name given to the set of extended properties.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcLabel Name
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _name;
            }
            set { this.SetModelValue(this, ref _name, value, v => Name = v, "Name"); }
        }

        #region Ifc Properties

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _extendedProperties.Add((IfcProperty) value.EntityVal);
                    break;
                case 2:
                    _description = value.StringVal;
                    break;
                case 3:
                    _name = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}