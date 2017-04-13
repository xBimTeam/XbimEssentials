using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Collections;
using Xbim.Ifc2x3.PresentationDefinitionResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcFaceTransient: PersistEntityTransient, Ifc4.Interfaces.IIfcFace
    {
        readonly IItemSet<Ifc4.Interfaces.IIfcFaceBound> _faceBounds;
        private Ifc4.Interfaces.IIfcFaceBound _bound;
        public IfcFaceTransient(IfcVertexBasedTextureMap textureMap)
        {
            _bound = new IfcFaceBoundTransient(textureMap.TexturePoints);
            _faceBounds = new ExtendedSingleSet<Ifc4.Interfaces.IIfcFaceBound,Ifc4.Interfaces.IIfcFaceBound>(
                () => _bound, 
                transient => _bound = transient,
                new ItemSet<Ifc4.Interfaces.IIfcFaceBound>(this, 0, 0), 
                bound => bound, bound => bound
                );
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
