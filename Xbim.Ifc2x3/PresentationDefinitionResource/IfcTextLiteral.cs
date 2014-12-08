#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTextLiteral.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.PresentationResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
    [IfcPersistedEntityAttribute]
    public class IfcTextLiteral : IfcGeometricRepresentationItem
    {
        #region Fields

        private IfcPresentableText _literal;
        private IfcAxis2Placement _placement;
        private IfcTextPath _path;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The text literal to be presented.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcPresentableText Literal
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _literal;
            }
            set { this.SetModelValue(this, ref _literal, value, v => Literal = v, "Literal"); }
        }

        /// <summary>
        ///   An IfcAxis2Placement that determines the placement and orientation of the presented string. 
        ///   When used with a text style based on TextStyleWithBoxCharacteristics then the y-axis is taken as the reference 
        ///   direction for the box rotation angle and the box slant angle.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcAxis2Placement Placement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _placement;
            }
            set { this.SetModelValue(this, ref _placement, value, v => Placement = v, "Placement"); }
        }

        /// <summary>
        ///   The writing direction of the text literal.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcTextPath Path
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _path;
            }
            set { this.SetModelValue(this, ref _path, value, v => Path = v, "Path"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _literal = value.StringVal;
                    break;
                case 1:
                    _placement = (IfcAxis2Placement) value.EntityVal;
                    break;
                case 2:
                    _path = (IfcTextPath) Enum.Parse(typeof (IfcTextPath), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            return "";
        }
    }
}