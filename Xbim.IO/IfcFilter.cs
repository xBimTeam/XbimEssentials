#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFilter.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Runtime.Serialization;

#endregion

namespace Xbim.IO
{
    [DataContract]
    public class IfcFilter
    {
        public IfcFilter(IfcType type)
        {
            _type = type;
        }

        public IfcFilter(IfcType type, params int[] propertyIndices)
            : this(type)
        {
            _propertyIndices = propertyIndices;
        }

        private IfcType _type;
        private int[] _propertyIndices;

        [DataMember]
        public int[] PropertyIndices
        {
            get { return _propertyIndices; }
            set { _propertyIndices = value; }
        }

        [DataMember]
        public IfcType Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}