using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Metadata;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.IO.Esent;

namespace Xbim.Extract
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceFile = "source.ifc";
            var copyFile = "copy.ifc";
            using (var source = new Xbim.Ifc2x3.IO.XbimModel())
            {
                PropertyTranformDelegate propTransform = delegate (ExpressMetaProperty prop, object toCopy)
                {
                    var value = prop.PropertyInfo.GetValue(toCopy, null);
                    return value;
                };
                source.CreateFrom(@"C:\Users\Steve\Downloads\Test Models\crash\NBS_LakesideRestaurant_EcoBuild2015_Revit2014_.ifc","source.xbim",null,true);

               // source.CreateFrom(@"C:\Users\Steve\Downloads\Test Models\Wall with complex openings.ifc", "source.xbim", null, true);
                // source.Open("BIM Logo-LetterM.xBIM");
                source.SaveAs(sourceFile);
                using (var target = Xbim.Ifc2x3.IO.XbimModel.CreateTemporaryModel())
                {
                    target.AutoAddOwnerHistory = false;
                    using (var txn = target.BeginTransaction())
                    {
                        target.Header = source.Header;
                        var copied = new XbimInstanceHandleMap(source, target);

                        foreach (var item in source.Instances.OfType<IfcProduct>())
                        {
                            var cpy = target.InsertCopy(item, copied, txn, propTransform, true);
                        }
                        txn.Commit();
                    }
                    target.SaveAs(copyFile);
                }
                source.Close();
                //the two files should be the same
               // FileCompare(sourceFile, copyFile);
            }
        }
    }
}
