#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcGeometricRepresentationContext.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;

using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.RepresentationResource
{
    /// <summary>
    ///   A geometric representation context is a representation context in which the geometric representation items are geometrically founded.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A geometric representation context is a representation context in which the geometric representation items are geometrically founded. A geometric representation context is a distinct coordinate space, spatially unrelated to other coordinate spaces. 
    ///   Definition from IAI: The IfcGeometricRepresentationContext defines the context that applies to several shape representations of products within a project. It defines the type of the context in which the shape representation is defined, and the numeric precision applicable to the geometric representation items defined in this context. In addition it can be used to offset the project coordinate system from a global point of origin, using the WorldCoordinateSystem attribute. The TrueNorth attribute can be given, if the y axis of the WorldCoordinateSystem does not point to the global northing. 
    ///   NOTE: The inherited attribute ContextType shall have one of the following recognized values: 'Sketch', 'Outline', 'Design', 'Detail', 'Model', 'Plan', 'NotDefined'.
    ///   The use of one instance of IfcGeometricRepresentationContext to represent the model (3D) view is mandatory, the use of a second instance of IfcGeometricRepresentationContext to represent the plan (2D) view is optional (but needs to be given, if there are scale dependent plan views), the additional scale or view dependent contexts need to be handled by using the subtype IfcGeometricRepresentationSubContext pointing to the model view (or the plan view) as the ParentContext. 
    ///   NOTE The definition of this class relates to the STEP entity geometric_representation_context. Please refer to ISO/IS 10303-42:1994 for the final definition of the formal standard. 
    ///   HISTORY New Entity in IFC Release 2.0
    ///   IFC2x Edition 3 CHANGE The attribute WorldCoordinateSystem has been made OPTIONAL Applicable values for ContextType are only 'Model',  'Plan', and  'NotDefined'. All other sub contexts are now handled by the new subtype in IFC2x Edition 2 IfcGeometricRepresentationSubContext. Upward compatibility for file based exchange is guaranteed.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcGeometricRepresentationContext : IfcRepresentationContext
    {
        #region Fields

        private IfcDimensionCount _coordinateSpaceDimension;
        private IfcReal? _precision;
        private IfcAxis2Placement _worldCoordinateSystem;
        private IfcDirection _trueNorth;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The integer dimension count of the coordinate space modeled in a geometric representation context.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public virtual IfcDimensionCount CoordinateSpaceDimension
        {
            get
            {
                IfcGeometricRepresentationSubContext subContext = this as IfcGeometricRepresentationSubContext;
                if (subContext != null)
                    return subContext.ParentContext.CoordinateSpaceDimension;
                else
                    return _coordinateSpaceDimension;
            }
            set
            {
                this.SetModelValue(this, ref _coordinateSpaceDimension, value, v => CoordinateSpaceDimension = v,
                                           "CoordinateSpaceDimension");
            }
        }

        /// <summary>
        ///   Optional. Value of the model precision for geometric models. It is a double value (REAL), typically in 1E-5 to 1E-8 range, that indicates the tolerance under which two given points are still assumed to be identical. The value can be used e.g. to sets the maximum distance from an edge curve to the underlying face surface in brep models.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public virtual IfcReal? Precision
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _precision;
            }
            set { this.SetModelValue(this, ref _precision, value, v => Precision = v, "Precision"); }
        }

        

        /// <summary>
        ///   Optional. Establishment of the engineering coordinate system (often refered to as the world coordinate system in CAD) for all representation contexts used by the project. If not given, it defaults to origin: (0.,0.,0.) and directions x(1.,0.,0.), y(0.,1.,0.), z(0.,0.,1.). Must be of type Axis2Placement2D or Axis2Placement3D
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public virtual IfcAxis2Placement WorldCoordinateSystem
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _worldCoordinateSystem;
            }
            set
            {
                if (value is IfcAxis2Placement2D || value is IfcAxis2Placement3D || value == null)
                    this.SetModelValue(this, ref _worldCoordinateSystem, value, v => WorldCoordinateSystem = v,
                                               "WorldCoordinateSystem");
                else
                    throw new ArgumentException(
                        "Illegal Axis2Placement type passed to GeometricRepresentationContext.WorldCoordinateSystem");
            }
        }


        /// <summary>
        ///   Optional. Direction of the true north relative to the underlying coordinate system as established by the attribute WorldCoordinateSystem. It is given by a direction within the xy-plane of the underlying coordinate system. If not given, it defaults to the positive direction of the y-axis of the WorldCoordinateSystem.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public virtual IfcDirection TrueNorth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _trueNorth;
            }
            set { this.SetModelValue(this, ref _trueNorth, value, v => TrueNorth = v, "TrueNorth"); }
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
                    _coordinateSpaceDimension = (sbyte) value.IntegerVal;
                    break;
                case 3:
                    _precision = value.RealVal;
                    break;
                case 4:
                    _worldCoordinateSystem = (IfcAxis2Placement) value.EntityVal;
                    break;
                case 5:
                    _trueNorth = (IfcDirection) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        /// <summary>
        /// Returns the default precision, if not define return 1e-005
        /// </summary>
        public double DefaultPrecision
        {
            get { return Precision ?? 1E-005;
                
            }
        }

        #endregion

        /// <summary>
        ///   Inverse. The set of IfcGeometricRepresentationSubContexts that refer to this IfcGeometricRepresentationContext.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcGeometricRepresentationSubContext> HasSubContexts
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcGeometricRepresentationSubContext>(
                        gsc => gsc.ParentContext == this);
            }
        }
    }
}