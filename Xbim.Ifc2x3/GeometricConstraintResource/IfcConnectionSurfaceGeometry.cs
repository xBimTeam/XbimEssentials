#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcConnectionSurfaceGeometry.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricConstraintResource
{
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcConnectionSurfaceGeometry : IfcConnectionGeometry
    {
        #region Fields

        private IfcSurfaceOrFaceSurface _surfaceOnRelatingElement;
        private IfcSurfaceOrFaceSurface _surfaceOnRelatedElement;

        #endregion

        #region Properties

        [IfcAttribute(1, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcSurfaceOrFaceSurface SurfaceOnRelatingElement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _surfaceOnRelatingElement;
            }
            set
            {
                this.SetModelValue(this, ref _surfaceOnRelatingElement, value, v => SurfaceOnRelatingElement = v,
                                           "SurfaceOnRelatingElement");
            }
        }

        [IfcAttribute(2, IfcAttributeState.Optional), IndexedProperty]
        public IfcSurfaceOrFaceSurface SurfaceOnRelatedElement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _surfaceOnRelatedElement;
            }
            set
            {
                this.SetModelValue(this, ref _surfaceOnRelatedElement, value, v => SurfaceOnRelatedElement = v,
                                           "SurfaceOnRelatedElement");
            }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _surfaceOnRelatingElement = (IfcSurfaceOrFaceSurface) value.EntityVal;
                    break;
                case 1:
                    _surfaceOnRelatedElement = (IfcSurfaceOrFaceSurface) value.EntityVal;
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