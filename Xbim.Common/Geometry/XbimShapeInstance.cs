using System.Collections.Generic;
using System.Linq;

namespace Xbim.Common.Geometry
{
    public class XbimShapeInstanceStyleGrouping : IGrouping<int, XbimShapeInstance>
    {
        private readonly IEnumerable<XbimShapeInstance> _shapeInstances;
        private readonly int _key;

        public XbimShapeInstanceStyleGrouping(int key, IEnumerable<XbimShapeInstance> shapeInstances)
        {
            _shapeInstances = shapeInstances;
            _key = key;
        }

        public IEnumerator<XbimShapeInstance> GetEnumerator()
        {
            return _shapeInstances.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _shapeInstances.GetEnumerator();
        }

        /// <summary>
        /// This is the label of the surface style
        /// </summary>
        int IGrouping<int, XbimShapeInstance>.Key
        {
            get { return _key; }
        }
    }

    /// <summary>
    /// A shape with a shapegeometry that has been placed as a specific instance in the scene
    /// i.e. it has had all maps performed and it has been transformed to the correct location
    /// This represents a geometry mesh, with a texture that is placed in world coordinate systems
    /// </summary>
    public class XbimShapeInstance : IXbimShapeInstanceData
    {
        int _instanceLabel;
        short _expressTypeId;
        int _ifcProductLabel;
        int _styleLabel;
        int _shapeLabel;
        int _representationContext;
        XbimGeometryRepresentationType _representationType;
        XbimMatrix3D _transformation;
        XbimRect3D _boundingBox;

        public XbimShapeInstance(int id = -1)
        {
            _instanceLabel = id;
            _expressTypeId = 0;
            _ifcProductLabel = 0;
            _styleLabel =0;
            _shapeLabel = -1;
            _representationContext = 0;
            _representationType = XbimGeometryRepresentationType.OpeningsAndAdditionsExcluded;
            _transformation = XbimMatrix3D.Identity;
            _boundingBox = XbimRect3D.Empty;
        }

        /// <summary>
        /// The unique label of this shape instance
        /// </summary>
        public int InstanceLabel
        {
            get
            {
                return _instanceLabel;
            }
            set
            {
                _instanceLabel=value;
            }
        }

        /// <summary>
        /// The IFC type (as short) of the product this instance represents
        /// </summary>
        public short IfcTypeId
        {
            get
            {
               return _expressTypeId;
            }
            set
            {
                _expressTypeId =  value;
            }
        }

        /// <summary>
        /// The label of the IFC Product that this instance defines (either partially or completely)
        /// </summary>
        public int IfcProductLabel
        {
            get
            {
                return _ifcProductLabel;
            }
            set
            {
                _ifcProductLabel = value;
            }
        }

        /// <summary>
        /// The style that this shape is presented in when it overrides the shape style
        /// </summary>
        public int StyleLabel
        {
            get
            {
                return _styleLabel;
            }
            set
            {
                _styleLabel = value;
            }
        }

        /// <summary>
        /// The pointer to the shape that this is an instance of
        /// </summary>
        public int ShapeGeometryLabel
        {
            get
            {
                return _shapeLabel;
            }
            set
            {
                _shapeLabel = value;
            }
        }

        /// <summary>
        /// The label of the IFC representation context of this instance
        /// </summary>
        public int RepresentationContext
        {
            get
            {
                return _representationContext;
            }
            set
            {
                _representationContext = value;
            }
        }

        /// <summary>
        /// What type of representation, typically this is how the shape has been generated
        /// (i.e. openings have been applied or not applied)
        /// </summary>
        public XbimGeometryRepresentationType RepresentationType
        {
            get
            {
                return _representationType;
            }
            set
            {
                _representationType = value;
            }
        }

        /// <summary>
        /// What type of representation, typically this is how the shape has been generated, i.e. openings have been applied or not applied
        /// </summary>
        byte IXbimShapeInstanceData.RepresentationType
        {
            get
            {
                return (byte)_representationType;
            }
            set
            {
                _representationType = (XbimGeometryRepresentationType)value;
            }
        }

        /// <summary>
        /// The transformation matrix to be applied to the shape so that the instance is placed relatively to the rest of the model
        /// </summary>
        public XbimMatrix3D Transformation
        {
            get
            {
                return _transformation;
            }
            set
            {
                _transformation = value;
            }
        }

        byte[] IXbimShapeInstanceData.Transformation
        {
            get
            {
                return _transformation.ToArray();
            }
            set
            {
                _transformation = XbimMatrix3D.FromArray(value);
            }
        }

        /// <summary>
        /// The bounding box of this instance, does not require tranformation to place in world coordinates
        /// </summary>
        public XbimRect3D BoundingBox
        {
            get
            {
                return _boundingBox;
            }
            set
            {
                _boundingBox = value;
            }
        }

        // todo: clarify if the bounding box needs transformation or not.
        // the information in the comments was conflicting.

        /// <summary>
        /// The bounding box of this instance, does not require tranformation to place in world coordinates
        /// </summary>
        byte[] IXbimShapeInstanceData.BoundingBox
        {
            get
            {
                return _boundingBox.ToFloatArray();
            }
            set
            {
                _boundingBox = XbimRect3D.FromArray(value);
            }
        }

        /// <summary>
        /// returns true if the shape instance has a defined style
        /// </summary>
        public bool HasStyle
        {
            get
            {
                return _styleLabel > 0;
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},TypeId: {2},{3},{4},{5},{6},{7}",  _instanceLabel,_styleLabel, _expressTypeId, _shapeLabel,_ifcProductLabel,  _representationContext, _representationType, _transformation.ToString());
        }      
    }
}
