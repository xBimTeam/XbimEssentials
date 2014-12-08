using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;

namespace Xbim.Ifc2x3.PresentationOrganizationResource
{
    /// <summary>
    /// The light source ambient entity is a subtype of light source. It lights a surface independent of the surface's orientation and position.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcLightSourceAmbient : IfcLightSource
    {
    }
}
