#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    ObjectDefinitionExtension.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Linq;
using Xbim.Ifc.Kernel;
using Xbim.XbimExtensions;
using Xbim.Ifc.SelectTypes;
using Xbim.Ifc.ProductExtension;
using Xbim.Ifc.MaterialResource;
using System;

#endregion

namespace Xbim.Ifc.Extensions
{
    public static class ObjectDefinitionExtension
    {
        public static void AddDecomposingObjectToFirstAggregation(this IfcObjectDefinition obj, IModel model,
                                                                  IfcObjectDefinition decomposingObject)
        {
            IfcRelAggregates rel =
                model.InstancesWhere<IfcRelAggregates>(r => r.RelatingObject == obj).FirstOrDefault() ??
                model.New<IfcRelAggregates>(r => r.RelatingObject = obj);

            rel.RelatedObjects.Add_Reversible(decomposingObject);
        }

       
       
    }
}