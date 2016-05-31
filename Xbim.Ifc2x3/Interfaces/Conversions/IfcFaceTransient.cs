using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc2x3.PresentationDefinitionResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcFaceTransient: PersistEntityTransient, Ifc4.Interfaces.IIfcFace
    {
        readonly List<IfcFaceBoundTransient> _faceBounds;
        public IfcFaceTransient(IfcVertexBasedTextureMap textureMap)
        {
            _faceBounds = new List<IfcFaceBoundTransient>(1) {new IfcFaceBoundTransient(textureMap.TexturePoints)};
        }
        public IItemSet<Ifc4.Interfaces.IIfcFaceBound> Bounds
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
