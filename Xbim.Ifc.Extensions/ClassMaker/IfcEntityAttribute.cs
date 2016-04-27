﻿#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcEntityAttribute.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Linq;
using Xbim.Common;
using Xbim.Common.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.XbimExtensions.ClassMaker
{
    public class IfcEntityAttribute
    {
        public IfcEntityAttribute(string propertyName, string typeName, EntityAttributeAttribute attributeAttributeProperties)
        {
            PropertyName = propertyName;
            TypeName = typeName;
            AttributeAttributeProperties = attributeAttributeProperties;
        }

        public EntityAttributeAttribute AttributeAttributeProperties;
        public string PropertyName;
        public string TypeName;

        public string FieldName
        {
            get
            {
                if (string.IsNullOrEmpty(PropertyName)) return PropertyName;
                char fc = PropertyName.ToLower().First();
                return "_" + fc + PropertyName.Substring(1);
            }
        }
    }
}