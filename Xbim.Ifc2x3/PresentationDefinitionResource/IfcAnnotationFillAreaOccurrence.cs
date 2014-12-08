#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAnnotationFillAreaOccurrence.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
    public class IfcAnnotationFillAreaOccurrence : IfcAnnotationOccurrence
    {
        private IfcPoint _fillStyleTarget;
        private IfcGlobalOrLocalEnum _globalOrLocal;

        /// <summary>
        ///   The point that specifies the starting location for the fill area style assigned to the annotation fill area occurrence.
        ///   Depending on the attribute GlobalOrLocal the point is either given within the world coordinate system of the project 
        ///   or within the object coordinate system of the element or annotation. If the FillStyleTarget is not given, it defaults to 0.,0.
        /// </summary>
        [Ifc(4, IfcAttributeState.Optional)]
        public IfcPoint FillStyleTarget
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _fillStyleTarget;
            }
            set
            {
                this.SetModelValue(this, ref _fillStyleTarget, value, v => FillStyleTarget = v,
                                           "FillStyleTarget");
            }
        }

        /// <summary>
        ///   The coordinate system in which the FillStyleTarget point is given. Depending on the attribute GlobalOrLocal the point 
        ///   is either given within the world coordinate system of the project or within the object coordinate system of the element 
        ///   or annotation. If not given, the hatch style is directly applied to the parameterization of the geometric representation item,
        ///   e.g. to the surface coordinate sytem, defined by the surface normal. 
        ///   ///
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public IfcGlobalOrLocalEnum GlobalOrLocal
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _globalOrLocal;
            }
            set { this.SetModelValue(this, ref _globalOrLocal, value, v => GlobalOrLocal = v, "GlobalOrLocal"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                    base.IfcParse(propIndex, value);
                    break;
                case 3:
                    _fillStyleTarget = (IfcPoint) value.EntityVal;
                    break;
                case 4:
                    _globalOrLocal = (IfcGlobalOrLocalEnum) Enum.Parse(typeof (IfcGlobalOrLocalEnum), value.EnumVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}