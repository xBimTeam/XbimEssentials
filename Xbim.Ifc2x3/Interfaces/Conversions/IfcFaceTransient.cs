using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcFaceTransient: PersistEntityTransient, Xbim.Ifc4.Interfaces.IIfcFace
    {
        Xbim.Ifc2x3.PresentationDefinitionResource.IfcVertexBasedTextureMap _textureMap;
        List<IfcFaceBoundTransient> _faceBounds;
        public IfcFaceTransient(Xbim.Ifc2x3.PresentationDefinitionResource.IfcVertexBasedTextureMap textureMap)
        {
            _textureMap = textureMap;
            _faceBounds = new List<IfcFaceBoundTransient>(1);
            _faceBounds.Add( new IfcFaceBoundTransient(_textureMap.TexturePoints));
        }
        public IEnumerable<Ifc4.Interfaces.IIfcFaceBound> Bounds
        {
            get { return _faceBounds; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcTextureMap> HasTextureMaps
        {
            get {return Enumerable.Empty<Ifc4.Interfaces.IIfcTextureMap>();}
        }

        public IEnumerable<Ifc4.Interfaces.IIfcPresentationLayerAssignment> LayerAssignment
        {
             get { return Enumerable.Empty<Ifc4.Interfaces.IIfcPresentationLayerAssignment>(); }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcStyledItem> StyledByItem
        {
             get { return Enumerable.Empty<Ifc4.Interfaces.IIfcStyledItem>(); }
        }
    }
}
