using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcTextureMapTransient : PersistEntityTransient, Xbim.Ifc4.Interfaces.IIfcTextureMap
    {
        Xbim.Ifc2x3.PresentationDefinitionResource.IfcTextureMap _textureMap;
        IfcFaceTransient _face;
        public IfcTextureMapTransient(Xbim.Ifc2x3.PresentationDefinitionResource.IfcTextureMap textureMap)
        {
            _textureMap = textureMap;
            if (textureMap.TextureMaps.Any())
            {
                _face = new IfcFaceTransient(textureMap.TextureMaps.First());
            }
            else
                throw new ArgumentException("IfcTextureMap should have at least one texture");
            
        }

        public IEnumerable<Ifc4.Interfaces.IIfcTextureVertex> Vertices
        {
            get
            {
                foreach (var textureVertex in _textureMap.TextureMaps.First().TextureVertices)
                {
                    yield return textureVertex;
                }
            }
        }

        public Ifc4.Interfaces.IIfcFace MappedTo
        {
            get
            {
                return _face;
            }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcSurfaceTexture> Maps
        {
            get 
            {
                //no idea where this is in ifc2x3
                return Enumerable.Empty<Ifc4.Interfaces.IIfcSurfaceTexture>();
            }
        }
    }
}
