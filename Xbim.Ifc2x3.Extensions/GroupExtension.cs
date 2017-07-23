using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.Kernel;

namespace Xbim.Ifc2x3.Extensions
{
    public static class GroupExtension
    {
        private static IModel GetModel(IPersistEntity root)
        {
            return root.Model;
        }

        public static void AddObjectToGroup(this IfcGroup gr, IfcObjectDefinition obj)
        {
            var model = GetModel(gr);

            IfcRelAssignsToGroup relation = gr.IsGroupedBy;
            if (gr.IsGroupedBy == null) relation = model.Instances.New<IfcRelAssignsToGroup>(rel => rel.RelatingGroup = gr);
            relation.RelatedObjects.Add(obj);
        }

        public static void AddObjectToGroup(this IfcGroup gr, IEnumerable<IfcObjectDefinition> objects)
        {
            var model = GetModel(gr);

            IfcRelAssignsToGroup relation = gr.IsGroupedBy;
            if (gr.IsGroupedBy == null) relation = model.Instances.New<IfcRelAssignsToGroup>(rel => rel.RelatingGroup = gr);
            foreach (var item in objects)
            {
                relation.RelatedObjects.Add(item);
            }
        }

        public static IEnumerable<IfcObjectDefinition> GetGroupedObjects(this IfcGroup gr)
        {
            IfcRelAssignsToGroup relation = gr.IsGroupedBy;
            if (gr.IsGroupedBy != null) return relation.RelatedObjects;
            return new List<IfcObjectDefinition>();
        }

        public static IEnumerable<T> GetGroupedObjects<T>(this IfcGroup gr) where T:IfcObjectDefinition
        {
            IfcRelAssignsToGroup relation = gr.IsGroupedBy;
            if (gr.IsGroupedBy != null) return relation.RelatedObjects.OfType<T>();
            return new List<T>();
        }

        public static IEnumerable<IfcGroup> GetParentGroups(this IfcGroup gr)
        {
            var model = GetModel(gr);

            var relations = model.Instances.Where<IfcRelAssignsToGroup>(rel => rel.RelatedObjects.Contains(gr));
            foreach (var rel in relations)
            {
                yield return rel.RelatingGroup;
            }
        }

        public static bool RemoveObjectFromGroup(this IfcGroup gr, IfcObjectDefinition obj)
        {
            if (gr == null || obj == null) return false;
            var model = GetModel(gr);
            IfcRelAssignsToGroup relation = gr.IsGroupedBy;
            if (gr.IsGroupedBy == null) return false;
            if (!relation.RelatedObjects.Contains(obj)) return false;
            relation.RelatedObjects.Remove(obj);
            return true;
        }
    }
}
