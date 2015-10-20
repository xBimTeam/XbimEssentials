#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    ObjectDefinitionExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

using System.Linq;
using Xbim.Common;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MaterialResource;
using Xbim.Ifc2x3.ProductExtension;


namespace Xbim.Ifc2x3.Extensions
{
    public static class ObjectDefinitionExtensions
    {
        public static void AddDecomposingObjectToFirstAggregation(this IfcObjectDefinition obj, IModel model,
                                                                 IfcObjectDefinition decomposingObject)
        {
            IfcRelAggregates rel =
                model.Instances.Where<IfcRelAggregates>(r => r.RelatingObject == obj).FirstOrDefault() ??
                model.Instances.New<IfcRelAggregates>(r => r.RelatingObject = obj);

            rel.RelatedObjects.Add(decomposingObject);
        }

        public static IfcMaterialSelect GetMaterial(this IfcObjectDefinition objDef)
        {
            IfcRelAssociatesMaterial relMat =  objDef.HasAssociations.OfType<IfcRelAssociatesMaterial>().FirstOrDefault();
            if (relMat != null)
                return relMat.RelatingMaterial;
            else
                return null;
        }
    }

}