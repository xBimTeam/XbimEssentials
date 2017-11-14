using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Ifc4
{
    public interface IXbimMeshCollectionReceiver
    {
        //adds a new mesh  
        IXbimMeshReceiver AddMesh();


    }
}
