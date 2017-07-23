#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    IfcClassificationExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.ExternalReferenceResource;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class ClassificationExtensions
    {
        public static IEnumerable<IfcClassificationItemRelationship> GetHierarchy(this IfcClassification cls)
        {
            IModel model = cls.Model;
            IEnumerable<IfcClassificationItemRelationship> itemRels =
                model.Instances.Where<IfcClassificationItemRelationship>(r => r.RelatingItem.ItemOf == cls);
            Dictionary<IfcClassificationItem, IfcClassificationItemRelationship> roots =
                new Dictionary<IfcClassificationItem, IfcClassificationItemRelationship>(); //top level items
            foreach (IfcClassificationItemRelationship itemRel in itemRels)
                roots.Add(itemRel.RelatingItem, itemRel); // so add it as a potential parent
            foreach (IfcClassificationItemRelationship itemRel in itemRels)
            {
                foreach (IfcClassificationItem child in itemRel.RelatedItems)
                {
                    if (roots.ContainsKey(child))
                        roots.Remove(child); //cannot be a child and a root
                }
            }
            return roots.Values;
        }
    }
}