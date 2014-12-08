using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
    /// <summary>
    /// An IfcTextureMap provides the mapping of the 2-dimensional texture coordinates to the surface onto which it is mapped. It is used for mapping the texture to vertex based geometry models, such as
    /// 
    ///     IfcFacetedBrep
    ///     IfcFacetedBrepWithVoids
    ///     IfcFaceBasedSurfaceModel
    ///     IfcShellBasedSurfaceModel
    /// 
    /// The IfcTextureMap provides a set of TextureMaps, each IfcVertexBasedTextureMap holds a corresponding pair of lists:
    /// 
    ///     a list of TexturePoints, currently of type IfcCartesianPoint, and
    ///     a list of TexturesVertices of type IfcTextureVertex.
    /// 
    /// Each IfcTextureVertex (given as S, T coordinates of 2 dimension) corresponds to the geometric coordinates of the IfcCartesianPoint (given as X, Y, and Z coordinates of 3 dimensions). 
    /// </summary>
    [IfcPersistedEntity]
    public class IfcTextureMap : IfcTextureCoordinate
    {
        public IfcTextureMap()
        {
            _TextureMaps = new XbimSet<IfcVertexBasedTextureMap>(this);
        }

        private XbimSet<IfcVertexBasedTextureMap> _TextureMaps;

        /// <summary>
        /// Reference to a list of texture vertex assignment to coordinates within a vertex based geometry. 
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public XbimSet<IfcVertexBasedTextureMap> TextureMaps
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _TextureMaps;
            }
            set { this.SetModelValue(this, ref _TextureMaps, value, v => TextureMaps = v, "TextureMaps"); }
        }

        public override string WhereRule()
        {
            var surface = AnnotatedSurface.FirstOrDefault();

            if (surface != null && 
                !(
                surface.Item is IfcShellBasedSurfaceModel ||
                surface.Item is IfcFaceBasedSurfaceModel ||
                surface.Item is IfcFacetedBrep ||
                surface.Item is IfcFacetedBrepWithVoids
                ))
                return "WR11:The texture map shall only be defined for an IfcAnnotatedSurface, referening a vertex based surface or solid model. \n";
            else
                return "";
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _TextureMaps.Add((IfcVertexBasedTextureMap)value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
