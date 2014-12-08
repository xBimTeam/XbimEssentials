using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.Ifc2x3.Kernel;

namespace Xbim.Ifc2x3.Extensions
{
    public static class ClassificationNotationSelectExtension
    {
        public static void AddObjectToClassificationNotation(this IfcClassificationNotationSelect cls, IfcRoot root)
        {
            var model = cls.ModelOf;
            var rel = model.Instances.Where<IfcRelAssociatesClassification>(r => r.RelatingClassification == cls).FirstOrDefault();
            if (rel == null)
                model.Instances.New<IfcRelAssociatesClassification>(r =>
                {
                    r.RelatingClassification = cls;
                    r.RelatedObjects.Add(root);
                });
            else if (!rel.RelatedObjects.Contains(root))
                rel.RelatedObjects.Add(root);
        }

        public static void RemoveObjectFromClassificationNotation(this IfcClassificationNotationSelect cls, IfcRoot root)
        {
            var model = cls.ModelOf;
            var rel = model.Instances.Where<IfcRelAssociatesClassification>(r => r.RelatingClassification == cls).FirstOrDefault();
            if (rel == null)
                return;
            else if (rel.RelatedObjects.Contains(root))
                rel.RelatedObjects.Remove(root);
        }
    }
}
