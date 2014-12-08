#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFilterDictionary.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.ObjectModel;

#endregion

namespace Xbim.IO
{
    [System.Serializable]
    public class IfcFilterDictionary : KeyedCollection<IfcType, IfcFilter>
    {
        protected override IfcType GetKeyForItem(IfcFilter item)
        {
            return item.Type;
        }
    }
}