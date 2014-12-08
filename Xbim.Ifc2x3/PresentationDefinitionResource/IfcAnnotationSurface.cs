using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
    /// <summary>
    /// An IfcAnnotationSurface is a surface or solid with texture coordinates assigned.
    /// It provides the capabilities to assign
    /// 
    ///     surface shading information,
    ///     surface rendering information
    ///     surface lighting information
    ///     surface textures
    /// 
    /// to a surface, or all surfaces of a face based surface model, a shell based surface model, 
    /// or a solid model. If the assigned IfcSurfaceStyle defines textures by including an instance 
    /// of IfcSurfaceStyleWithTextures, the attribute TextureCoordinates determines the mapping of the 
    /// texture to the surface(s) of the Item. In case of vertex based geometry, texture maps may be 
    /// used to define the texture coordinates for each face. 
    /// 
    /// The style information is linked by using the IfcStyledItem to the IfcAnnotationSurface instance.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcAnnotationSurface : IfcGeometricRepresentationItem
    {
        #region fields
        IfcGeometricRepresentationItem _item;
        IfcTextureCoordinate _textureCoordinates;
        #endregion

        /// <summary>
        /// Geometric representation item, providing the geometric definition
        /// of the annotated surface. It is further restricted to be a surface, 
        /// surface model, or solid model. 
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcGeometricRepresentationItem Item
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _item;
            }
            set { this.SetModelValue(this, ref _item, value, v => Item = v, "Item"); }
        }

        /// <summary>
        ///  Texture coordinates, such as a texture map, that are associated 
        ///  with the textures for the surface style. It should only be given, 
        ///  if the IfcSurfaceStyle associated to the IfcAnnotationSurfaceOccurrence 
        ///  contains an IfcSurfaceStyleWithTextures. 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcTextureCoordinate TextureCoordinates
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _textureCoordinates;
            }
            set { this.SetModelValue(this, ref _textureCoordinates, value, v => TextureCoordinates = v, "TextureCoordinates"); }
        }

        public override void IfcParse(int propIndex, XbimExtensions.Interfaces.IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _item = (IfcGeometricRepresentationItem)value.EntityVal;
                    break;
                case 1:
                    _textureCoordinates = (IfcTextureCoordinate)value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            var result = "";

            if (!(
                Item is IfcSurface ||
                Item is IfcShellBasedSurfaceModel ||
                Item is IfcFaceBasedSurfaceModel ||
                Item is IfcSolidModel ||
                Item is IfcBooleanResult ||
                Item is IfcCsgPrimitive3D
                ))
                result += "WR01: Only surfaces, surface models, solids and 3D primitives and CSG results are applicable as Items. |n";

            return result;
        }
    }
}
