using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.IO;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.GeometryResource;
using System.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem(@"TestSourceFiles\")]
    public class ModelFilterTest
    {
        [TestMethod]
        public void CopyAllEntitiesTest()
        {
            using (var source = new XbimModel())
            {
                PropertyTranformDelegate propTransform = delegate(IfcMetaProperty prop, object toCopy)
                {              
                    var value = prop.PropertyInfo.GetValue(toCopy, null);
                    return value;
                };



                source.Open("BIM Logo-LetterM.xBIM");
                source.SaveAs("WithGeometry.ifc");
                using (var target = XbimModel.CreateTemporaryModel())
                {
                    target.AutoAddOwnerHistory = false;
                    using (var txn = target.BeginTransaction())
                    {
                        var copied = new XbimInstanceHandleMap(source, target);

                        foreach (var item in source.Instances)
                        {
                            target.InsertCopy(item, copied, txn, propTransform);
                        }
                        txn.Commit();
                    }
                    target.SaveAs("WithoutGeometry.ifc");
                }
                source.Close();
                //the two files should be the same
            }
        }
        
        [TestMethod]
        public void ExtractIfcGeometryEntitiesTest()
        {
            using (var source = new XbimModel())
            {
                PropertyTranformDelegate propTransform = delegate(IfcMetaProperty prop, object toCopy)
                {

                    if (typeof(IfcProduct).IsAssignableFrom(toCopy.GetType()))
                    {
                        if (prop.PropertyInfo.Name == "ObjectPlacement" || prop.PropertyInfo.Name == "Representation")
                            return null;
                    }   
                    if(typeof(IfcTypeProduct).IsAssignableFrom(toCopy.GetType()))
                    {
                        if (prop.PropertyInfo.Name == "RepresentationMaps" )
                            return null;
                    }
                    return prop.PropertyInfo.GetValue(toCopy, null);//just pass through the value               
                };

                //source.Open("BIM Logo-LetterM.xBIM");
                //source.SaveAs("WithGeometry.ifc");
                string modelName = @"4walls1floorSite";
                string xbimModelName = Path.ChangeExtension(modelName,"xbim");
                
                source.CreateFrom( Path.ChangeExtension(modelName,"ifc"), null, null, true);
               
                using (var target = XbimModel.CreateModel(Path.ChangeExtension(modelName + "_NoGeom", "xbim")))
                {
                    target.AutoAddOwnerHistory = false;
                    using (var txn = target.BeginTransaction())
                    {
                        var copied = new XbimInstanceHandleMap(source, target);

                        foreach (var item in source.Instances.OfType<IfcRoot>())
                        {
                            target.InsertCopy(item, copied, txn, propTransform, false);
                        }
                        txn.Commit();
                    }
                    
                    target.SaveAs(Path.ChangeExtension(modelName + "_NoGeom", "ifc"));
                    target.Close();
                    
                }
                
                source.Close();
               // XbimModel.Compact(Path.ChangeExtension(modelName + "_NoGeom", "xbim"), Path.ChangeExtension(modelName + "_NoGeom_Compacted", "xbim"));
                //the two files should be the same
            }
        }
    }
}
